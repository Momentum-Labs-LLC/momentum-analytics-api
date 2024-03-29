using Microsoft.AspNetCore.Mvc;
using Momentum.Analytics.Core;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.PageViews.Interfaces;
using Momentum.Analytics.Lambda.Api.Cookies;
using Momentum.Analytics.Lambda.Api.PageViews.ViewModels;
using Momentum.FromCookie;
using NodaTime;

namespace Momentum.Analytics.Lambda.Api.PageViews
{
    [ApiController]
    [Route("v1/page-views")]
    public class PageViewsController : ControllerBase
    {
        protected readonly IPageViewService _pageViewService;
        protected readonly ICookieWriter _cookieWriter;
        protected readonly IClockService _clockService;
        protected readonly IVisitExpirationProvider _visitExpirationProvider;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly ILogger _logger;

        public PageViewsController(
            IPageViewService pageViewService, 
            ICookieWriter cookieWriter, 
            IClockService clockService,
            IVisitExpirationProvider visitExpirationProvider,
            ILogger<PageViewsController> logger)
        {
            _pageViewService = pageViewService ?? throw new ArgumentNullException(nameof(pageViewService));
            _cookieWriter = cookieWriter ?? throw new ArgumentNullException(nameof(cookieWriter));
            _clockService = clockService ?? throw new ArgumentNullException(nameof(clockService));
            _visitExpirationProvider = visitExpirationProvider ?? throw new ArgumentNullException(nameof(visitExpirationProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        [HttpPost]
        public async Task<IActionResult> PageViewedAsync(
            [FromBody] PageViewViewModel pageView,
            [FromCookie(Name = CookieConstants.NAME)] string? cookieValue = null,
            CancellationToken token = default)
        {
            IActionResult result = Ok();

            try
            {
                var now = _clockService.Now;
                var visitExpiration = await _visitExpirationProvider.GetExpirationAsync(now, token).ConfigureAwait(false);
                var cookie = cookieValue.ToCookieModel(visitExpiration);
                
                var domainModel = pageView.ToDomain(cookie, now);
                await _pageViewService.RecordAsync(domainModel, token).ConfigureAwait(false);

                if(cookie.MaxFunnelStep < pageView.FunnelStep)
                {
                    cookie.MaxFunnelStep = pageView.FunnelStep;
                } // end if

                await _cookieWriter.SetCookieAsync(cookie, token).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                _logger.LogError(new EventId(0), ex, "Failed to accept page view.");
                result = StatusCode(500);
            } // end try/catch

            return result;
        } // end method
    } // end class
} // end namespace