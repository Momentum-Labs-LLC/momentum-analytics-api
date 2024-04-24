using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Models;
using Momentum.Analytics.Core.PII;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.PII.Models;
using Moq;
using NodaTime;
using NodaTime.Extensions;

namespace Momentum.Analytics.Core.Tests.Pii
{
    public class PiiServiceTests 
    {
        private Mock<ICollectedPiiStorage<int, ISearchResponse<CollectedPii>>> _collectedPiiStorage;
        private Mock<IPiiValueStorage> _piiValueStorage;
        private Mock<IEmailHasher> _emailHasher;
        private Mock<ILogger<TestPiiService>> _logger;
        private TestPiiService _service;

        public PiiServiceTests()
        {
            _collectedPiiStorage = new Mock<ICollectedPiiStorage<int, ISearchResponse<CollectedPii>>>();
            _piiValueStorage = new Mock<IPiiValueStorage>();
            _emailHasher = new Mock<IEmailHasher>();
            _logger = new Mock<ILogger<TestPiiService>>();

            _service = new TestPiiService(_collectedPiiStorage.Object, _piiValueStorage.Object, _emailHasher.Object, _logger.Object);
        } // end method

        protected ISearchResponse<CollectedPii> BuildCollectedPiiSearchResponse(Guid cookieId, int count, int nextPage = 0)
        {
            var response = new SearchResponse<CollectedPii>()
            {
                Data = Enumerable.Range(0, count).Select(x => new CollectedPii()
                {
                    PiiId = Guid.NewGuid(),
                    CookieId = cookieId,
                    UtcTimestamp = DateTime.UtcNow.ToInstant().Minus(Duration.FromMinutes(x)),
                }),
                NextPage = nextPage
            };

            return response;
        } // end method

        [Fact]
        public async Task GetUniqueUserIdsAsync_Single()
        {
            var cookieId = Guid.NewGuid();

            _collectedPiiStorage.Setup(x => x.GetLatestUserIdsAsync(cookieId, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(BuildCollectedPiiSearchResponse(cookieId, 1));

            _piiValueStorage.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PiiValue()
                {
                    Value = "12123",
                    PiiType = PiiTypeEnum.UserId
                });

            var results = await _service.GetUniqueUserIdsAsync(cookieId).ConfigureAwait(false);

            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.Equal(1, results.Count());

            _collectedPiiStorage.Verify(x => x.GetLatestUserIdsAsync(cookieId, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            _piiValueStorage.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
        } // end method

        [Fact]
        public async Task GetUniqueUserIdsAsync_Eleven_OutOf_10()
        {
            var random = new Random();
            var maximum = 10;
            var cookieId = Guid.NewGuid();

            _collectedPiiStorage.SetupSequence(x => x.GetLatestUserIdsAsync(cookieId, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(BuildCollectedPiiSearchResponse(cookieId, 6, 2))
                .ReturnsAsync(BuildCollectedPiiSearchResponse(cookieId, 5));

            _piiValueStorage.Setup(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PiiValue()
                {
                    Value = random.Next(0, 100000).ToString(),
                    PiiType = PiiTypeEnum.UserId
                });

            var results = await _service.GetUniqueUserIdsAsync(cookieId, maximum).ConfigureAwait(false);

            Assert.NotNull(results);
            Assert.NotEmpty(results);
            Assert.Equal(maximum, results.Count());

            _collectedPiiStorage.Verify(x => x.GetLatestUserIdsAsync(cookieId, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _piiValueStorage.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Exactly(maximum));
        } // end method
    
        [Fact]
        public async Task RecordAsync_NewUserId()
        {
            var piiId = Guid.NewGuid();
            var collectedPii = new CollectedPii()
            {
                CookieId = Guid.NewGuid(),
                UtcTimestamp = DateTime.UtcNow.ToInstant(),
                Pii = new PiiValue()
                {
                    PiiType = PiiTypeEnum.UserId,
                    Value = "12345"
                }
            };

            _piiValueStorage.Setup(x => x.GetByValueAsync(collectedPii.Pii.Value, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PiiValue)null);
            _piiValueStorage.Setup(x => x.InsertAsync(It.IsAny<PiiValue>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _collectedPiiStorage.Setup(x => x.InsertAysnc(It.IsAny<CollectedPii>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _service.RecordAsync(collectedPii).ConfigureAwait(false);

            _emailHasher.Verify(x => x.HashEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _piiValueStorage.Verify(x => x.GetByValueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _piiValueStorage.Verify(x => x.InsertAsync(It.IsAny<PiiValue>(), It.IsAny<CancellationToken>()), Times.Once);
            _collectedPiiStorage.Verify(x => x.InsertAysnc(It.IsAny<CollectedPii>(), It.IsAny<CancellationToken>()), Times.Once);
        } // end method

        [Fact]
        public async Task RecordAsync_OldUserId()
        {
            var piiId = Guid.NewGuid();
            var collectedPii = new CollectedPii()
            {
                CookieId = Guid.NewGuid(),
                UtcTimestamp = DateTime.UtcNow.ToInstant(),
                Pii = new PiiValue()
                {
                    PiiType = PiiTypeEnum.UserId,
                    Value = "12345"
                }
            };

            _piiValueStorage.Setup(x => x.GetByValueAsync(collectedPii.Pii.Value, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PiiValue()
                {
                    Id = piiId,
                    Value = collectedPii.Pii.Value,
                    PiiType = PiiTypeEnum.UserId,
                    UtcTimestamp = DateTime.UtcNow.ToInstant().Minus(Duration.FromHours(1))
                });

            _collectedPiiStorage.Setup(x => x.InsertAysnc(It.IsAny<CollectedPii>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _service.RecordAsync(collectedPii).ConfigureAwait(false);

            _emailHasher.Verify(x => x.HashEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
            _piiValueStorage.Verify(x => x.GetByValueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _piiValueStorage.Verify(x => x.InsertAsync(It.IsAny<PiiValue>(), It.IsAny<CancellationToken>()), Times.Never);
            _collectedPiiStorage.Verify(x => x.InsertAysnc(It.IsAny<CollectedPii>(), It.IsAny<CancellationToken>()), Times.Once);
        } // end method

        [Fact]
        public async Task RecordAsync_OldEmail()
        {
            var piiId = Guid.NewGuid();
            var collectedPii = new CollectedPii()
            {
                CookieId = Guid.NewGuid(),
                UtcTimestamp = DateTime.UtcNow.ToInstant(),
                Pii = new PiiValue()
                {
                    PiiType = PiiTypeEnum.Email,
                    Value = "abc@gmail.com"
                }
            };

            var hashValue = "asd;flkansdfaop;isdnfaposdifn";
            _emailHasher.Setup(x => x.HashEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(hashValue);

            _piiValueStorage.Setup(x => x.GetByValueAsync(hashValue, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PiiValue()
                {
                    Id = piiId,
                    Value = collectedPii.Pii.Value,
                    PiiType = PiiTypeEnum.UserId,
                    UtcTimestamp = DateTime.UtcNow.ToInstant().Minus(Duration.FromHours(1))
                });

            _collectedPiiStorage.Setup(x => x.InsertAysnc(It.IsAny<CollectedPii>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _service.RecordAsync(collectedPii).ConfigureAwait(false);

            _emailHasher.Verify(x => x.HashEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _piiValueStorage.Verify(x => x.GetByValueAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
            _piiValueStorage.Verify(x => x.InsertAsync(It.IsAny<PiiValue>(), It.IsAny<CancellationToken>()), Times.Never);
            _collectedPiiStorage.Verify(x => x.InsertAysnc(It.IsAny<CollectedPii>(), It.IsAny<CancellationToken>()), Times.Once);
        } // end method
    } // end class

    public class TestPiiService : PiiService<int, ISearchResponse<CollectedPii>, ICollectedPiiStorage<int, ISearchResponse<CollectedPii>>>
    {
        public TestPiiService(
                ICollectedPiiStorage<int, ISearchResponse<CollectedPii>> collectedPiiStorage, 
                IPiiValueStorage piiValueStorage, 
                IEmailHasher emailHasher, 
                ILogger<TestPiiService> logger) 
            : base(collectedPiiStorage, piiValueStorage, emailHasher, logger)
        {

        } // end method
    } // end class
} // end namespace