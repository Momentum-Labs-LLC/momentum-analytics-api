using System.Text;
using System.Text.Json;
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
        protected readonly IVisitWindowCalculator _visitWindowCalculator;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly ILogger _logger;

        public PageViewsController(
            IPageViewService pageViewService, 
            ICookieWriter cookieWriter, 
            IClockService clockService,
            IVisitWindowCalculator visitWindowCalculator,
            ILogger<PageViewsController> logger)
        {
            _pageViewService = pageViewService ?? throw new ArgumentNullException(nameof(pageViewService));
            _cookieWriter = cookieWriter ?? throw new ArgumentNullException(nameof(cookieWriter));
            _clockService = clockService ?? throw new ArgumentNullException(nameof(clockService));
            _visitWindowCalculator = visitWindowCalculator ?? throw new ArgumentNullException(nameof(visitWindowCalculator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        [HttpGet(ApiConstants.PIXEL_PATH)]
        public async Task<IActionResult> PageViewedPixelAsync(
            [FromQuery(Name = ApiConstants.QUERY_STRING_EVENT_KEY)] string base64PageView,
            [FromCookie(Name = CookieConstants.NAME)] string? cookieValue = null,
            CancellationToken token = default)
        {
            IActionResult result = File(ApiConstants.GIF_BYTES, "image/gif");

            PageViewViewModel viewModel;
            var pageViewBytes = Convert.FromBase64String(base64PageView);
            using (var stream = new MemoryStream(pageViewBytes))
            {
                viewModel = await JsonSerializer.DeserializeAsync<PageViewViewModel>(stream, cancellationToken: token).ConfigureAwait(false);
            } // end using

            if(viewModel != null)
            {
                try
                {
                    await AcceptPageViewAsync(viewModel, cookieValue, token).ConfigureAwait(false);
                }
                catch(Exception ex)
                {
                    _logger.LogError(new EventId(0), ex, "Failed to accept page view.");
                    result = StatusCode(500);
                } // end try/catch
            }
            else
            {
                result = BadRequest();
            } // end if

            return result;
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
                await AcceptPageViewAsync(pageView, cookieValue, token).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                _logger.LogError(new EventId(0), ex, "Failed to accept page view.");
                result = StatusCode(500);
            } // end try/catch

            return result;
        } // end method

        // [HttpPost("/builder")]
        // public async Task<IActionResult> BuildPixelAsync(
        //     [FromBody] PageViewViewModel pageView,
        //     CancellationToken token = default)
        // {
        //     var json = JsonSerializer.Serialize(pageView);

        //     var base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

        //     return Ok(base64String);
        // } // end method

        protected async Task AcceptPageViewAsync(
            PageViewViewModel viewModel, 
            string? cookieValue = null,
            CancellationToken token = default)
        {
            var now = _clockService.Now;
            var visitExpiration = await _visitWindowCalculator.GetExpirationAsync(now, token).ConfigureAwait(false);
            var cookie = cookieValue.ToCookieModel(visitExpiration);
            
            var domainModel = viewModel.ToDomain(cookie, now);
            await _pageViewService.RecordAsync(domainModel, token).ConfigureAwait(false);

            if(cookie.MaxFunnelStep < domainModel.FunnelStep)
            {
                cookie.MaxFunnelStep = domainModel.FunnelStep;
            } // end if

            await _cookieWriter.SetCookieAsync(cookie, token).ConfigureAwait(false);
        } // end method
    } // end class
} // end namespace