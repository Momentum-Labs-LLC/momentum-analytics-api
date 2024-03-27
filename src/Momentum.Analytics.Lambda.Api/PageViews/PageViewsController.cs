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
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly ILogger _logger;

        public PageViewsController(
            IPageViewService pageViewService, 
            ICookieWriter cookieWriter, 
            IClockService clockService,
            IHttpContextAccessor httpContextAccessor,
            ILogger<PageViewsController> logger)
        {
            _pageViewService = pageViewService ?? throw new ArgumentNullException(nameof(pageViewService));
            _cookieWriter = cookieWriter ?? throw new ArgumentNullException(nameof(cookieWriter));
            _clockService = clockService ?? throw new ArgumentNullException(nameof(clockService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        [HttpPost]
        public async Task<IActionResult> PageViewedAsync(
            [FromBody] PageViewViewModel pageView,
            [FromCookie(Name = CookieConstants.NAME)] string? cookieValue = null,
            CancellationToken token = default)
        {
            var cookie = cookieValue.ToCookieModel();
            var now = _clockService.Now;
            var domainModel = pageView.ToDomain(cookie, now);
            await _pageViewService.RecordAsync(domainModel, token).ConfigureAwait(false);

            if(cookie.MaxFunnelStep < pageView.FunnelStep)
            {
                cookie.MaxFunnelStep = pageView.FunnelStep;
            } // end if

            await _cookieWriter.SetCookieAsync(cookie, token).ConfigureAwait(false);

            return Ok();
        } // end method
    } // end class
} // end namespace