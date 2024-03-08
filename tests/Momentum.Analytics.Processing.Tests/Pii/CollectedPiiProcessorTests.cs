using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.PageViews.Interfaces;
using Momentum.Analytics.Core.PageViews.Models;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.Core.Visits;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Processing.Pii;
using Moq;

namespace Momentum.Analytics.Processing.Tests.Pii
{
    public class CollectedPiiProcessorTests
    {
        private Mock<IPageViewService> _pageViewService;
        private Mock<IPiiService> _piiService;
        private Mock<IIdentifiedVisitService> _visitService;        
        private Mock<ILogger<CollectedPiiProcessor>> _logger;
        private CollectedPiiProcessor _processor;

        public CollectedPiiProcessorTests()
        {
            _pageViewService = new Mock<IPageViewService>();
            _piiService = new Mock<IPiiService>();
            _visitService = new Mock<IIdentifiedVisitService>();
            _logger = new Mock<ILogger<CollectedPiiProcessor>>();

            _processor = new CollectedPiiProcessor(
                _pageViewService.Object,
                _piiService.Object,
                _visitService.Object,
                _logger.Object);
        } // end method

        protected PiiValue BuildUserId(string userId)
        {
            return new PiiValue()
            {
                Id = Guid.NewGuid(),
                Value = userId,
                PiiType = PiiTypeEnum.UserId,
                HashAlgorithm = HashAlgorithmEnum.None
            };
        } // end method

        protected PiiValue BuildEmail(string emailHash)
        {
            return new PiiValue()
            {
                Id = Guid.NewGuid(),
                Value = emailHash,
                PiiType = PiiTypeEnum.Email,
                HashAlgorithm = HashAlgorithmEnum.SHA256
            };
        } // end method

        protected CollectedPii BuildCollectedPii(PiiValue piiValue)
        {
            return new CollectedPii()
            {
                Id = Guid.NewGuid(),
                CookieId = Guid.NewGuid(),
                UtcTimestamp = DateTime.UtcNow,
                Pii = piiValue,
            };
        } // end method

        protected PageView BuildPageView(CollectedPii collectedPii, DateTime utcTimestamp)
        {
            return new PageView()
            {
                RequestId = Guid.NewGuid().ToString(),
                CookieId = collectedPii.CookieId,
                UtcTimestamp = utcTimestamp,
                Domain = "test.com",
                Path = "index",
                ReferrerDomain = "google.com",
                FunnelStep = 0
            };
        } // end method

        [Fact]
        public async Task ProcessAsync_NoPii_NewUserId_NewIdentifiedVisit()
        {
            var userId = BuildUserId("12345");
            var collectedPii = BuildCollectedPii(userId);
            _piiService.Setup(x => x.GetByCookieIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PiiValue>() { userId });
            
            var pageView = BuildPageView(collectedPii, collectedPii.UtcTimestamp.AddMinutes(-1));
            _pageViewService.Setup(x => x.GetByCookieAsync(collectedPii.CookieId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PageView>() 
                {
                    pageView
                });

            _visitService.Setup(x => x.CreateVisitAsync(collectedPii.Pii, It.IsAny<IEnumerable<PageView>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _processor.ProcessAsync(collectedPii).ConfigureAwait(false);
            
            _piiService.Verify(x => x.GetByCookieIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            _pageViewService.Verify(x => x.GetByCookieAsync(collectedPii.CookieId, It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.CreateVisitAsync(collectedPii.Pii, It.IsAny<IEnumerable<PageView>>(), It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.GetActiveVisitAsync(It.IsAny<PiiValue>(), It.IsAny<CancellationToken>()), Times.Never);
            _visitService.Verify(x => x.UpdatePiiAsync(It.IsAny<IdentifiedVisit>(), It.IsAny<PiiValue>(), It.IsAny<CancellationToken>()), Times.Never);
        } // end method

        [Fact]
        public async Task ProcessAsync_NoPii_NewUserId_NoPageViews()
        {
            var userId = BuildUserId("12345");
            var collectedPii = BuildCollectedPii(userId);
            _piiService.Setup(x => x.GetByCookieIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PiiValue>() { userId });
            
            _pageViewService.Setup(x => x.GetByCookieAsync(collectedPii.CookieId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PageView>() {});

            _visitService.Setup(x => x.CreateVisitAsync(collectedPii.Pii, It.IsAny<IEnumerable<PageView>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _processor.ProcessAsync(collectedPii).ConfigureAwait(false);
            
            _piiService.Verify(x => x.GetByCookieIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            _pageViewService.Verify(x => x.GetByCookieAsync(collectedPii.CookieId, It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.CreateVisitAsync(collectedPii.Pii, It.IsAny<IEnumerable<PageView>>(), It.IsAny<CancellationToken>()), Times.Never);
            _visitService.Verify(x => x.GetActiveVisitAsync(It.IsAny<PiiValue>(), It.IsAny<CancellationToken>()), Times.Never);
            _visitService.Verify(x => x.UpdatePiiAsync(It.IsAny<IdentifiedVisit>(), It.IsAny<PiiValue>(), It.IsAny<CancellationToken>()), Times.Never);
        } // end method

        [Fact]
        public async Task ProcessAsync_ExistingValue()
        {
            var userId = BuildUserId("12345");
            var sameUserId = BuildUserId("12345");
            var collectedPii = BuildCollectedPii(userId);
            _piiService.Setup(x => x.GetByCookieIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PiiValue>() { userId, sameUserId });

            await _processor.ProcessAsync(collectedPii).ConfigureAwait(false);
            
            _piiService.Verify(x => x.GetByCookieIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            _pageViewService.Verify(x => x.GetByCookieAsync(collectedPii.CookieId, It.IsAny<CancellationToken>()), Times.Never);
            _visitService.Verify(x => x.CreateVisitAsync(collectedPii.Pii, It.IsAny<IEnumerable<PageView>>(), It.IsAny<CancellationToken>()), Times.Never);
            _visitService.Verify(x => x.GetActiveVisitAsync(It.IsAny<PiiValue>(), It.IsAny<CancellationToken>()), Times.Never);
            _visitService.Verify(x => x.UpdatePiiAsync(It.IsAny<IdentifiedVisit>(), It.IsAny<PiiValue>(), It.IsAny<CancellationToken>()), Times.Never);
        } // end method

        [Fact]
        public async Task ProcessAsync_NewUserId_UpdatedPii()
        {
            var email = BuildEmail("abc@test.com");
            var userId = BuildUserId("12345");

            var collectedPii = BuildCollectedPii(userId);
            _piiService.Setup(x => x.GetByCookieIdAsync(collectedPii.CookieId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PiiValue>() { userId, email });
            
            var pageView = BuildPageView(collectedPii, collectedPii.UtcTimestamp.AddMinutes(-1));
            _pageViewService.Setup(x => x.GetByCookieAsync(collectedPii.CookieId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PageView>() 
                {
                    pageView
                });

            var visit = new IdentifiedVisit()
                {
                    PiiValue = email.Value,
                    PiiType = email.PiiType,
                    UtcVisitStart = pageView.UtcTimestamp,
                    UtcTimestamp = pageView.UtcTimestamp
                };
            _visitService.Setup(x => x.GetActiveVisitAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(visit);

            _visitService.Setup(x => x.UpdatePiiAsync(visit, userId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _processor.ProcessAsync(collectedPii).ConfigureAwait(false);
            
            _piiService.Verify(x => x.GetByCookieIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Once);
            _pageViewService.Verify(x => x.GetByCookieAsync(collectedPii.CookieId, It.IsAny<CancellationToken>()), Times.Never);
            _visitService.Verify(x => x.CreateVisitAsync(collectedPii.Pii, It.IsAny<IEnumerable<PageView>>(), It.IsAny<CancellationToken>()), Times.Never);
            _visitService.Verify(x => x.GetActiveVisitAsync(It.IsAny<PiiValue>(), It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.UpdatePiiAsync(It.IsAny<IdentifiedVisit>(), It.IsAny<PiiValue>(), It.IsAny<CancellationToken>()), Times.Once);
        } // end method
    } // end class
} // end namespace