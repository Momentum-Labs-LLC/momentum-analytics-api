using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.PageViews.Models;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.Core.Visits;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Processing.PageViews;
using Moq;

namespace Momentum.Analytics.Processing.Tests.PageViews
{
    public class PageViewProcessorTests
    {
        private Mock<IPiiService> _piiService;
        private Mock<IIdentifiedVisitService> _visitService;
        private Mock<ILogger<PageViewProcessor>> _logger;
        private PageViewProcessor _processor;

        public PageViewProcessorTests()
        {
            _piiService = new Mock<IPiiService>();
            _visitService = new Mock<IIdentifiedVisitService>();
            _logger = new Mock<ILogger<PageViewProcessor>>();
            _processor = new PageViewProcessor(_piiService.Object, _visitService.Object, _logger.Object);
        } // end method

        protected PageView BuildPageView(string? path = null, string? referrer = null, int funnelStep = 0)
        {
            return new PageView()
            {
                RequestId = Guid.NewGuid().ToString(),
                CookieId = Guid.NewGuid(),
                UtcTimestamp = DateTime.UtcNow,
                Domain = "test.com",
                Path = "index",
                ReferrerDomain = "google.com",
                FunnelStep = funnelStep
            };
        } // end method

        [Fact]
        public async Task ProcessAsync_NoPii()
        {
            var pageView = BuildPageView();

            _piiService.Setup(x => x.GetByCookieIdAsync(pageView.CookieId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PiiValue>());

            await _processor.ProcessAsync(pageView).ConfigureAwait(false);

            _piiService.Verify(x => x.GetByCookieIdAsync(pageView.CookieId, It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.GetActiveVisitAsync(It.IsAny<PiiValue>(), It.IsAny<CancellationToken>()), Times.Never);
            _visitService.Verify(x => x.UpdateFunnelStepAsync(It.IsAny<IdentifiedVisit>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _visitService.Verify(x => x.CreateVisitAsync(It.IsAny<PiiValue>(), It.IsAny<IEnumerable<PageView>>(), It.IsAny<CancellationToken>()), Times.Never);
        } // end method

        [Fact]
        public async Task ProcessAsync_NewVisit()
        {
            var pageView = BuildPageView();

            var userId = new PiiValue()
                {
                    Id = Guid.NewGuid(),
                    PiiType = PiiTypeEnum.UserId,
                    Value = "12345",
                    HashAlgorithm = HashAlgorithmEnum.None
                };
            _piiService.Setup(x => x.GetByCookieIdAsync(pageView.CookieId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PiiValue>() 
                {
                    userId
                });

            _visitService.Setup(x => x.GetActiveVisitAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((IdentifiedVisit)null);

            _visitService.Setup(x => x.CreateVisitAsync(userId, It.IsAny<IEnumerable<PageView>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _processor.ProcessAsync(pageView).ConfigureAwait(false);

            _piiService.Verify(x => x.GetByCookieIdAsync(pageView.CookieId, It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.GetActiveVisitAsync(It.IsAny<PiiValue>(), It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.UpdateFunnelStepAsync(It.IsAny<IdentifiedVisit>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _visitService.Verify(x => x.CreateVisitAsync(It.IsAny<PiiValue>(), It.IsAny<IEnumerable<PageView>>(), It.IsAny<CancellationToken>()), Times.Once);
        } // end method

        [Fact]
        public async Task ProcessAsync_ExistingVisit_NoUpdate()
        {
            var pageView = BuildPageView();

            var userId = new PiiValue()
                {
                    Id = Guid.NewGuid(),
                    PiiType = PiiTypeEnum.UserId,
                    Value = "12345",
                    HashAlgorithm = HashAlgorithmEnum.None
                };
            _piiService.Setup(x => x.GetByCookieIdAsync(pageView.CookieId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PiiValue>() 
                {
                    userId
                });

            _visitService.Setup(x => x.GetActiveVisitAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new IdentifiedVisit()
                {
                    PiiValue = userId.Value,
                    PiiType = userId.PiiType,
                    UtcTimestamp = pageView.UtcTimestamp.AddMinutes(-2),
                    FunnelStep = 0
                });

            await _processor.ProcessAsync(pageView).ConfigureAwait(false);

            _piiService.Verify(x => x.GetByCookieIdAsync(pageView.CookieId, It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.GetActiveVisitAsync(It.IsAny<PiiValue>(), It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.UpdateFunnelStepAsync(It.IsAny<IdentifiedVisit>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
            _visitService.Verify(x => x.CreateVisitAsync(It.IsAny<PiiValue>(), It.IsAny<IEnumerable<PageView>>(), It.IsAny<CancellationToken>()), Times.Never);
        } // end method

        [Fact]
        public async Task ProcessAsync_ExistingVisit_UpdateFunnelStep()
        {
            var pageView = BuildPageView(funnelStep: 1);

            var userId = new PiiValue()
                {
                    Id = Guid.NewGuid(),
                    PiiType = PiiTypeEnum.UserId,
                    Value = "12345",
                    HashAlgorithm = HashAlgorithmEnum.None
                };
            _piiService.Setup(x => x.GetByCookieIdAsync(pageView.CookieId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<PiiValue>() 
                {
                    userId
                });

            _visitService.Setup(x => x.GetActiveVisitAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new IdentifiedVisit()
                {
                    PiiValue = userId.Value,
                    PiiType = userId.PiiType,
                    UtcTimestamp = pageView.UtcTimestamp.AddMinutes(-2),
                    FunnelStep = 0
                });

            await _processor.ProcessAsync(pageView).ConfigureAwait(false);

            _piiService.Verify(x => x.GetByCookieIdAsync(pageView.CookieId, It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.GetActiveVisitAsync(It.IsAny<PiiValue>(), It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.UpdateFunnelStepAsync(It.IsAny<IdentifiedVisit>(), It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.CreateVisitAsync(It.IsAny<PiiValue>(), It.IsAny<IEnumerable<PageView>>(), It.IsAny<CancellationToken>()), Times.Never);
        } // end method
    } // end class
} // end namespace