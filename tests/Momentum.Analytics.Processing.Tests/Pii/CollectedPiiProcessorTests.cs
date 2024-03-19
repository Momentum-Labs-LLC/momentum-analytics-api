using System.Net;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Models;
using Momentum.Analytics.Core.PageViews.Interfaces;
using Momentum.Analytics.Core.PageViews.Models;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.Processing.Pii;
using Moq;

namespace Momentum.Analytics.Processing.Tests.Pii
{
    public class CollectedPiiProcessorTests
    {
        private Mock<ITestVisitService> _visitService;        
        private Mock<ILogger<TestCollectedPiiProcessor>> _logger;
        private TestCollectedPiiProcessor _processor;

        public CollectedPiiProcessorTests()
        {
            _visitService = new Mock<ITestVisitService>();
            _logger = new Mock<ILogger<TestCollectedPiiProcessor>>();

            _processor = new TestCollectedPiiProcessor(
                _visitService.Object,
                _logger.Object);
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
                UtcTimestamp = DateTime.UtcNow,
                Pii = piiValue,
            };
        } // end method

        protected Visit BuildVisit(Guid cookieId, DateTime utcTimestamp)
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

            _visitService.Setup(x => x.GetUnidentifiedAsync(collectedPii.CookieId, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SearchResponse<Visit>() { Total = 0, HasMore = false, Data = null });            

            await _processor.ProcessAsync(collectedPii).ConfigureAwait(false);
            
            _visitService.Verify(x => x.GetByActivityAsync(It.IsAny<IUserActivity>(), It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.UpsertAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.GetUnidentifiedAsync(collectedPii.CookieId, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        } // end method

        [Fact]
        public async Task ProcessAsync_ActiveVisit_Identified_Improved()
        {
            var userId = BuildUserId("12345");
            var collectedPii = BuildCollectedPii(userId);
            
            var activeVisit = BuildVisit(collectedPii.CookieId, collectedPii.UtcTimestamp);
            activeVisit.PiiType = PiiTypeEnum.Email;
            activeVisit.PiiValue = "asdfasdfasdf";
            activeVisit.UtcIdentifiedTimestamp = DateTime.UtcNow.AddMinutes(-5);
            _visitService.Setup(x => x.GetByActivityAsync(It.IsAny<IUserActivity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(activeVisit);

            _visitService.Setup(x => x.UpsertAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _visitService.Setup(x => x.GetUnidentifiedAsync(collectedPii.CookieId, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SearchResponse<Visit>() { Total = 0, HasMore = false, Data = null });            

            await _processor.ProcessAsync(collectedPii).ConfigureAwait(false);
            
            _visitService.Verify(x => x.GetByActivityAsync(It.IsAny<IUserActivity>(), It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.UpsertAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.GetUnidentifiedAsync(collectedPii.CookieId, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        } // end method

        [Fact]
        public async Task ProcessAsync_ActiveUnidentified_OldUnidentified()
        {
            var userId = BuildUserId("12345");
            var collectedPii = BuildCollectedPii(userId);
            
            var activeVisit = BuildVisit(collectedPii.CookieId, collectedPii.UtcTimestamp);
            activeVisit.PiiType = PiiTypeEnum.Email;
            activeVisit.PiiValue = "asdfasdfasdf";
            activeVisit.UtcIdentifiedTimestamp = DateTime.UtcNow.AddMinutes(-5);
            _visitService.Setup(x => x.GetByActivityAsync(It.IsAny<IUserActivity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(activeVisit);

            _visitService.Setup(x => x.UpsertAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _visitService.SetupSequence(x => x.GetUnidentifiedAsync(collectedPii.CookieId, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SearchResponse<Visit>() 
                { 
                    Total = 2, 
                    HasMore = true, 
                    Data = new List<Visit>() { activeVisit } 
                })
                .ReturnsAsync(new SearchResponse<Visit>() 
                { 
                    Total = 2,
                    HasMore = false, 
                    Data = new List<Visit>() { BuildVisit(collectedPii.CookieId, DateTime.UtcNow.AddDays(-2)) } 
                });

            await _processor.ProcessAsync(collectedPii).ConfigureAwait(false);
            
            _visitService.Verify(x => x.GetByActivityAsync(It.IsAny<IUserActivity>(), It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.UpsertAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
            _visitService.Verify(x => x.GetUnidentifiedAsync(collectedPii.CookieId, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
        } // end method
    
        [Fact]
        public async Task ProcessAsync_ActiveIdentified_NotImproved()
        {
            var userId = BuildEmail("abc@123.com");
            var collectedPii = BuildCollectedPii(userId);
            
            var activeVisit = BuildVisit(collectedPii.CookieId, collectedPii.UtcTimestamp);
            activeVisit.PiiType = PiiTypeEnum.UserId;
            activeVisit.PiiValue = "12345";
            activeVisit.UtcIdentifiedTimestamp = DateTime.UtcNow.AddMinutes(-5);
            _visitService.Setup(x => x.GetByActivityAsync(It.IsAny<IUserActivity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(activeVisit);

            _visitService.Setup(x => x.UpsertAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            _visitService.Setup(x => x.GetUnidentifiedAsync(collectedPii.CookieId, It.IsAny<int>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SearchResponse<Visit>() { Total = 0, HasMore = false, Data = null });            

            await _processor.ProcessAsync(collectedPii).ConfigureAwait(false);
            
            _visitService.Verify(x => x.GetByActivityAsync(It.IsAny<IUserActivity>(), It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.UpsertAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()), Times.Never);
            _visitService.Verify(x => x.GetUnidentifiedAsync(collectedPii.CookieId, It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        } // end method
    } // end class
} // end namespace