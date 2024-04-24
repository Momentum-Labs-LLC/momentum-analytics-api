using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Models;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.Processing.Cookies;
using Momentum.Analytics.Processing.Visits;
using Momentum.Analytics.Processing.Visits.Interfaces;
using Moq;
using NodaTime.Extensions;

namespace Momentum.Analytics.Processing.Tests.Visits
{
    public class UnidentifiedVisitProcessorTests
    {
        private Mock<IVisitService<int, ISearchResponse<Visit>>> _visitService;
        private Mock<IPiiService> _piiService;
        private Mock<ISharedCookieConfiguration> _sharedCookieConfiguration;
        private Mock<ILogger<TestUnidentifiedVisitProcessor>> _logger;
        private TestUnidentifiedVisitProcessor _processor;
        public UnidentifiedVisitProcessorTests()
        {
            _visitService = new Mock<IVisitService<int, ISearchResponse<Visit>>>();
            _piiService = new Mock<IPiiService>();
            _sharedCookieConfiguration = new Mock<ISharedCookieConfiguration>();
            _logger = new Mock<ILogger<TestUnidentifiedVisitProcessor>>();

            _processor = new TestUnidentifiedVisitProcessor(
                _visitService.Object,
                _piiService.Object,
                _sharedCookieConfiguration.Object,
                _logger.Object
            );
            
            _sharedCookieConfiguration.Setup(x => x.Threshold).Returns(5);
        } // end method

        [Fact]
        public async Task ProcessAsync_None()
        {
            _visitService.Setup(x => x.GetUnidentifiedAsync(It.IsAny<ITimeRange>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SearchResponse<Visit>() { Total = 0 });
            
            var timeRange = new TimeRange() 
            {
                UtcStart = DateTime.UtcNow.ToInstant(),
                UtcEnd = DateTime.UtcNow.ToInstant()
            };             
            
            await _processor.ProcessAsync(timeRange).ConfigureAwait(false);

            _visitService.Verify(x => x.GetUnidentifiedAsync(It.IsAny<ITimeRange>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            _piiService.Verify(x => x.GetUniqueUserIdsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _sharedCookieConfiguration.Verify(x => x.Threshold, Times.Never);
        } // end method

        [Fact]
        public async Task ProcessAsync_OneResult_NoUserIds()
        {
            var visits = new List<Visit>()
            {
                new Visit()
                {
                    Id = Guid.NewGuid(),
                    CookieId = Guid.NewGuid(),
                    UtcStart = DateTime.UtcNow.ToInstant(),
                    UtcExpiration = DateTime.UtcNow.ToInstant(),
                }
            };

            _visitService.Setup(x => x.GetUnidentifiedAsync(It.IsAny<ITimeRange>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SearchResponse<Visit>() { Total = 1, Data = visits });
            _piiService.Setup(x => x.GetUniqueUserIdsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CollectedPii>());
            
            var timeRange = new TimeRange() 
            {
                UtcStart = DateTime.UtcNow.ToInstant(),
                UtcEnd = DateTime.UtcNow.ToInstant()
            };             
            
            await _processor.ProcessAsync(timeRange).ConfigureAwait(false);

            _visitService.Verify(x => x.GetUnidentifiedAsync(It.IsAny<ITimeRange>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            _piiService.Verify(x => x.GetUniqueUserIdsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.UpsertAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()), Times.Never);
        } // end method

        [Fact]
        public async Task ProcessAsync_OneResult_FoundUserId()
        {
            var visits = new List<Visit>()
            {
                new Visit()
                {
                    Id = Guid.NewGuid(),
                    CookieId = Guid.NewGuid(),
                    UtcStart = DateTime.UtcNow.ToInstant(),
                    UtcExpiration = DateTime.UtcNow.ToInstant(),
                }
            };

            _visitService.Setup(x => x.GetUnidentifiedAsync(It.IsAny<ITimeRange>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SearchResponse<Visit>() { Total = 1, Data = visits });
            _piiService.Setup(x => x.GetUniqueUserIdsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CollectedPii>()
                {
                    new CollectedPii()
                    {
                        Pii = new PiiValue()
                        {
                            Value = "12345"
                        }
                    }
                });
            
            var timeRange = new TimeRange() 
            {
                UtcStart = DateTime.UtcNow.ToInstant(),
                UtcEnd = DateTime.UtcNow.ToInstant()
            };             
            
            await _processor.ProcessAsync(timeRange).ConfigureAwait(false);

            _visitService.Verify(x => x.GetUnidentifiedAsync(It.IsAny<ITimeRange>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            _piiService.Verify(x => x.GetUniqueUserIdsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.UpsertAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()), Times.Once);
        } // end method

        protected new List<Visit> BuildUnidentifiedVisits(int count)
        {
            var result = new List<Visit>();
            for(int i = 0; i < count; i++)
            {
                result.Add(new Visit()
                {
                    Id = Guid.NewGuid(),
                    CookieId = Guid.NewGuid(),
                    UtcStart = DateTime.UtcNow.ToInstant(),
                    UtcExpiration = DateTime.UtcNow.ToInstant(),
                });
            }

            return result;
        }

        [Fact]
        public async Task ProcessAsync_MultiplePages()
        {

            _visitService.SetupSequence(x => x.GetUnidentifiedAsync(It.IsAny<ITimeRange>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SearchResponse<Visit>() { Total = 1, Data = BuildUnidentifiedVisits(1), NextPage = 2 })
                .ReturnsAsync(new SearchResponse<Visit>() { Total = 1, Data = BuildUnidentifiedVisits(1) });
            _piiService.Setup(x => x.GetUniqueUserIdsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CollectedPii>()
                {
                    new CollectedPii()
                    {
                        Pii = new PiiValue()
                        {
                            Value = "12345"
                        }
                    }
                });
            
            var timeRange = new TimeRange() 
            {
                UtcStart = DateTime.UtcNow.ToInstant(),
                UtcEnd = DateTime.UtcNow.ToInstant()
            };             
            
            await _processor.ProcessAsync(timeRange).ConfigureAwait(false);

            _visitService.Verify(x => x.GetUnidentifiedAsync(It.IsAny<ITimeRange>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _piiService.Verify(x => x.GetUniqueUserIdsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _visitService.Verify(x => x.UpsertAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        } // end method
    } // end class

    public class TestUnidentifiedVisitProcessor : UnidentifiedVisitProcessor<int, ISearchResponse<Visit>, IVisitService<int, ISearchResponse<Visit>>>
    {
        public TestUnidentifiedVisitProcessor(
            IVisitService<int, ISearchResponse<Visit>> visitService, 
            IPiiService piiService, 
            ISharedCookieConfiguration sharedCookieConfiguration, 
            ILogger<TestUnidentifiedVisitProcessor> logger) 
            : base(visitService, piiService, sharedCookieConfiguration, logger)
        {
        } // end method
    } // end class
} // end namespace