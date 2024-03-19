using Microsoft.AspNetCore.Mvc;
using Momentum.Analytics.Core.PageViews.Interfaces;
using Momentum.Analytics.Lambda.Api.Cookies;
using Momentum.Analytics.Lambda.Api.PageViews.ViewModels;
using Momentum.FromCookie;

namespace Momentum.Analytics.Lambda.Api.PageViews
{
    [ApiController]
    [Route("v1/page-views")]
    public class PageViewsController : ControllerBase
    {
        protected readonly IPageViewService _pageViewService;
        protected readonly ICookieWriter _cookieWriter;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly ILogger _logger;

        public PageViewsController(
            IPageViewService pageViewService, 
            ICookieWriter cookieWriter, 
            IHttpContextAccessor httpContextAccessor,
            ILogger<PageViewsController> logger)
        {
            _pageViewService = pageViewService ?? throw new ArgumentNullException(nameof(pageViewService));
            _cookieWriter = cookieWriter ?? throw new ArgumentNullException(nameof(cookieWriter));
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
            var domainModel = pageView.ToDomain(cookie, _httpContextAccessor.HttpContext.TraceIdentifier);
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