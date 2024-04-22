using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.PageViews.Models;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.Processing.Cookies;
using Momentum.Analytics.Processing.PageViews;
using Moq;
using NodaTime;

namespace Momentum.Analytics.Processing.Tests.PageViews
{
    public class PageViewProcessorTests
    {
        private IClockService _clockService;
        private Mock<ITestVisitService> _visitService;
        private Mock<ISharedCookieConfiguration> _sharedCookieConfiguration;
        private Mock<ILogger<TestPageViewProcessor>> _logger;
        private TestPageViewProcessor _processor;

        public PageViewProcessorTests()
        {
            _clockService = new ClockService();
            _visitService = new Mock<ITestVisitService>();
            _sharedCookieConfiguration = new Mock<ISharedCookieConfiguration>();
            _logger = new Mock<ILogger<TestPageViewProcessor>>();
            _processor = new TestPageViewProcessor(_visitService.Object, _sharedCookieConfiguration.Object, _clockService, _logger.Object);
        } // end method

        protected PageView BuildPageView(string? path = null, string? referrer = null, int funnelStep = 0)
        {
            return new PageView()
            {
                CookieId = Guid.NewGuid(),
                UtcTimestamp = _clockService.Now,
                Domain = "test.com",
                Path = "index",
                Referer = "google.com",
                FunnelStep = funnelStep
            };
        } // end method

        [Fact]
        public async Task ProcessAsync_VisitExists_NoPii()
        {
            var pageView = BuildPageView();

            _visitService.Setup(x => x.GetByActivityAsync(It.IsAny<IUserActivity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Visit()
                {
                    Id = Guid.NewGuid(),
                    CookieId = pageView.CookieId,
                    Referer = pageView.Referer,
                    FunnelStep = pageView.FunnelStep,
                    UtcStart = pageView.UtcTimestamp.Minus(Duration.FromMinutes(1)),
                    UtcExpiration = pageView.UtcTimestamp.Plus(Duration.FromDays(1))
                });

            await _processor.ProcessAsync(pageView).ConfigureAwait(false);

            _visitService.Verify(x => x.GetByActivityAsync(It.IsAny<IUserActivity>(), It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.UpsertAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()), Times.Never);
        } // end method

        [Fact]
        public async Task ProcessAsync_NewVisit()
        {
            var pageView = BuildPageView();

            var userId = new PiiValue()
                {
                    Id = Guid.NewGuid(),
                    PiiType = PiiTypeEnum.UserId,
                    Value = "12345"
                };

            _visitService.Setup(x => x.GetByActivityAsync(It.IsAny<IUserActivity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((Visit)null);

            _visitService.Setup(x => x.UpsertAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            await _processor.ProcessAsync(pageView).ConfigureAwait(false);

            _visitService.Verify(x => x.GetByActivityAsync(It.IsAny<IUserActivity>(), It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.UpsertAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()), Times.Once);
        } // end method

        [Fact]
        public async Task ProcessAsync_ExistingVisit_NoUpdate()
        {
            var pageView = BuildPageView();

            var userId = new PiiValue()
                {
                    Id = Guid.NewGuid(),
                    PiiType = PiiTypeEnum.UserId,
                    Value = "12345"
                };

            _visitService.Setup(x => x.GetByActivityAsync(It.IsAny<IUserActivity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Visit()
                {
                    Id = Guid.NewGuid(),
                    CookieId = Guid.NewGuid(),
                    PiiValue = userId.Value,
                    PiiType = userId.PiiType,
                    UtcIdentifiedTimestamp = pageView.UtcTimestamp.Minus(Duration.FromMinutes(2)),
                    FunnelStep = 0
                });

            await _processor.ProcessAsync(pageView).ConfigureAwait(false);
            
            _visitService.Verify(x => x.GetByActivityAsync(It.IsAny<IUserActivity>(), It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.UpsertAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()), Times.Once);            
        } // end method

        [Fact]
        public async Task ProcessAsync_ExistingVisit_UpdateFunnelStep()
        {
            var pageView = BuildPageView(funnelStep: 1);

            var userId = new PiiValue()
                {
                    Id = Guid.NewGuid(),
                    PiiType = PiiTypeEnum.UserId,
                    Value = "12345"
                };

            _visitService.Setup(x => x.GetByActivityAsync(It.IsAny<IUserActivity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Visit()
                {
                    Id= Guid.NewGuid(),
                    CookieId = Guid.NewGuid(),
                    PiiValue = userId.Value,
                    PiiType = userId.PiiType,
                    UtcIdentifiedTimestamp = pageView.UtcTimestamp.Minus(Duration.FromMinutes(2)),
                });

            await _processor.ProcessAsync(pageView).ConfigureAwait(false);

            _visitService.Verify(x => x.GetByActivityAsync(It.IsAny<IUserActivity>(), It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.UpsertAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()), Times.Once);
        } // end method

        [Fact]
        public async Task ProcessAsync_ExistingVisit_WithUserId()
        {
            var pageView = BuildPageView();

            _visitService.Setup(x => x.GetByActivityAsync(It.IsAny<IUserActivity>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Visit()
                {
                    Id = Guid.NewGuid(),
                    CookieId = pageView.CookieId,
                    Referer = pageView.Referer,
                    FunnelStep = pageView.FunnelStep,
                    UtcStart = pageView.UtcTimestamp.Minus(Duration.FromMinutes(1)),
                    UtcExpiration = pageView.UtcTimestamp.Plus(Duration.FromDays(1)),
                    PiiType = PiiTypeEnum.UserId,
                    PiiValue = "12345",
                    UtcIdentifiedTimestamp = pageView.UtcTimestamp.Minus(Duration.FromMinutes(1))
                });

            await _processor.ProcessAsync(pageView).ConfigureAwait(false);

            _visitService.Verify(x => x.GetByActivityAsync(It.IsAny<IUserActivity>(), It.IsAny<CancellationToken>()), Times.Once);
            _visitService.Verify(x => x.UpsertAsync(It.IsAny<Visit>(), It.IsAny<CancellationToken>()), Times.Never);
        } // end method
    } // end class
} // end namespace