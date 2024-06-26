using System.Net;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Models;
using Momentum.Analytics.Core.PageViews.Interfaces;
using Momentum.Analytics.Core.PageViews.Models;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.Processing.Cookies;
using Momentum.Analytics.Processing.Pii;
using Moq;
using NodaTime;

namespace Momentum.Analytics.Processing.Tests.Pii
{
    public class CollectedPiiProcessorTests
    {
        private IClockService _clockService;
        private Mock<ITestVisitService> _visitService;
        private Mock<IVisitWindowCalculator> _visitWindowCalculator;
        private Mock<IPiiService> _piiService;
        private Mock<ISharedCookieConfiguration> _sharedCookieConfiguration;
        private Mock<ILogger<TestCollectedPiiProcessor>> _logger;
        private TestCollectedPiiProcessor _processor;

        public CollectedPiiProcessorTests()
        {
            _clockService = new ClockService();
            _visitService = new Mock<ITestVisitService>();
            _visitWindowCalculator = new Mock<IVisitWindowCalculator>();
            _piiService = new Mock<IPiiService>();
            _sharedCookieConfiguration = new Mock<ISharedCookieConfiguration>();
            _logger = new Mock<ILogger<TestCollectedPiiProcessor>>();

            _processor = new TestCollectedPiiProcessor(
                _visitService.Object,
                _clockService,
                _visitWindowCalculator.Object,
                _piiService.Object,
                _sharedCookieConfiguration.Object,
                _logger.Object);

            _sharedCookieConfiguration.Setup(x => x.Threshold).Returns(5);
            _piiService.Setup(x => x.GetUniqueUserIdsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CollectedPii>());
        } // end method

        protected PiiValue BuildUserId(string userId)
        {
            return new PiiValue()
            {
                Id = Guid.NewGuid(),
                Value = userId,
                PiiType = PiiTypeEnum.UserId
            };
        } // end method

        protected PiiValue BuildEmail(string emailHash)
        {
            return new PiiValue()
            {
                Id = Guid.NewGuid(),
                Value = emailHash,
                PiiType = PiiTypeEnum.Email
            };
        } // end method

        protected CollectedPii BuildCollectedPii(PiiValue piiValue)
        {
            return new CollectedPii()
            {
                PiiId = Guid.NewGuid(),
                CookieId = Guid.NewGuid(),
                UtcTimestamp = _clockService.Now,
                Pii = piiValue,
            };
        } // end method

        protected Visit BuildVisit(Guid cookieId, Instant utcTimestamp)
        {
            return new Visit()
            {
                Id = Guid.NewGuid(),
                CookieId = cookieId,
                UtcStart = utcTimestamp,
                Referer = "test.com",
                FunnelStep = 0
            };
        } // end method

        [Fact]
        public async Task ProcessAsync_NoActiveVisit_NoUnidentifiedVisits()
        {
            var userId = BuildUserId("12345");
            var collectedPii = BuildCollectedPii(userId);
            
            _visitService.Setup(x => x.GetByActivityAsync(It.IsAny<IUserActivity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Visit)null);

            _visitService.Setup(x => x.UpsertAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _piiService.Setup(x => x.GetUniqueUserIdsAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<CollectedPii>());

            _visitService.Setup(x => x.GetUnidentifiedAsync(collectedPii.CookieId, collectedPii.UtcTimestamp, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SearchResponse<Visit>() { Total = 0, Data = null });            

            await _processor.ProcessAsync(collectedPii).ConfigureAwait(false);
            
            _visitService.Verify(x => x.GetByActivityAsync(It.IsAny<IUserActivity>(), It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.UpsertAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.GetUnidentifiedAsync(collectedPii.CookieId, collectedPii.UtcTimestamp, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        } // end method

        [Fact]
        public async Task ProcessAsync_ActiveVisit_Identified_Improved()
        {
            var now = _clockService.Now;
            var userId = BuildUserId("12345");
            var collectedPii = BuildCollectedPii(userId);
            
            var activeVisit = BuildVisit(collectedPii.CookieId, collectedPii.UtcTimestamp);
            activeVisit.PiiType = PiiTypeEnum.Email;
            activeVisit.PiiValue = "asdfasdfasdf";
            activeVisit.UtcIdentifiedTimestamp = now.Minus(Duration.FromMinutes(-5));
            _visitService.Setup(x => x.GetByActivityAsync(It.IsAny<IUserActivity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(activeVisit);

            _visitService.Setup(x => x.UpsertAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _visitService.Setup(x => x.GetUnidentifiedAsync(collectedPii.CookieId, collectedPii.UtcTimestamp, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SearchResponse<Visit>() { Total = 0, Data = null });            

            await _processor.ProcessAsync(collectedPii).ConfigureAwait(false);
            
            _visitService.Verify(x => x.GetByActivityAsync(It.IsAny<IUserActivity>(), It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.UpsertAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.GetUnidentifiedAsync(collectedPii.CookieId, collectedPii.UtcTimestamp, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        } // end method

        [Fact]
        public async Task ProcessAsync_ActiveUnidentified_OldUnidentified()
        {
            var now = _clockService.Now;
            var userId = BuildUserId("12345");
            var collectedPii = BuildCollectedPii(userId);
            
            var activeVisit = BuildVisit(collectedPii.CookieId, collectedPii.UtcTimestamp);
            _visitService.Setup(x => x.GetByActivityAsync(It.IsAny<IUserActivity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(activeVisit);

            _visitService.Setup(x => x.UpsertAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _visitService.SetupSequence(x => x.GetUnidentifiedAsync(collectedPii.CookieId, collectedPii.UtcTimestamp, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SearchResponse<Visit>() 
                { 
                    Data = new List<Visit>() { activeVisit },
                    NextPage = 2
                })
                .ReturnsAsync(new SearchResponse<Visit>() 
                { 
                    Data = new List<Visit>() { BuildVisit(collectedPii.CookieId, now.Minus(Duration.FromDays(2))) } 
                });

            await _processor.ProcessAsync(collectedPii).ConfigureAwait(false);
            
            _visitService.Verify(x => x.GetByActivityAsync(It.IsAny<IUserActivity>(), It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.UpsertAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _visitService.Verify(x => x.GetUnidentifiedAsync(collectedPii.CookieId, collectedPii.UtcTimestamp, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        } // end method
    
        [Fact]
        public async Task ProcessAsync_ActiveIdentified_NotImproved()
        {
            var now = _clockService.Now;
            var userId = BuildEmail("abc@123.com");
            var collectedPii = BuildCollectedPii(userId);
            
            var activeVisit = BuildVisit(collectedPii.CookieId, collectedPii.UtcTimestamp);
            activeVisit.PiiType = PiiTypeEnum.UserId;
            activeVisit.PiiValue = "12345";
            activeVisit.UtcIdentifiedTimestamp = now.Minus(Duration.FromMinutes(5));
            _visitService.Setup(x => x.GetByActivityAsync(It.IsAny<IUserActivity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(activeVisit);

            _visitService.Setup(x => x.UpsertAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _visitService.Setup(x => x.GetUnidentifiedAsync(collectedPii.CookieId, collectedPii.UtcTimestamp, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SearchResponse<Visit>() { Total = 0, Data = null });            

            await _processor.ProcessAsync(collectedPii).ConfigureAwait(false);
            
            _visitService.Verify(x => x.GetByActivityAsync(It.IsAny<IUserActivity>(), It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.UpsertAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()), Times.Never);
            _visitService.Verify(x => x.GetUnidentifiedAsync(collectedPii.CookieId, collectedPii.UtcTimestamp, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        } // end method
    } // end class
} // end namespace