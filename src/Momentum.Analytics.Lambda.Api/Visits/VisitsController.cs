using Microsoft.AspNetCore.Mvc;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Models;
using Momentum.Analytics.Core.PageViews.Models;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.DynamoDb.Visits.Interfaces;
using Momentum.Analytics.Lambda.Api.Cookies;
using Momentum.FromCookie;
using NodaTime;

namespace Momentum.Analytics.Lambda.Api.Visits
{
    [ApiController]
    [Route("v1/visits")]
    public class VisitsController : ControllerBase
    {
        protected readonly IDynamoDbVisitService _visitService;
        protected readonly IClockService _clockService;

        protected readonly ILogger _logger;

        public VisitsController(
            IDynamoDbVisitService visitService, 
            IClockService clockService,
            ILogger<VisitsController> logger)
        {
            _visitService = visitService ?? throw new ArgumentNullException(nameof(visitService));
            _clockService = clockService ?? throw new ArgumentNullException(nameof(clockService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        // [HttpGet]
        // public async Task<IActionResult> GetActiveVisitAsync(
        //     [FromCookie(Name = CookieConstants.NAME)] string cookieValue,
        //     [FromQuery(Name = "cookieId")] Guid? cookieId = null,
        //     CancellationToken token = default)
        // {
        //     var now = _clockService.Now;
        //     var visitExpiration = await _visitService.GetVisitExpirationAsync(now, token).ConfigureAwait(false);
        //     var cookie = cookieValue.ToCookieModel(visitExpiration);
        //     if(cookieId.HasValue)
        //     {
        //         cookie.Id = cookieId.Value;
        //     } // end if

        //     var pageView = new PageView() { CookieId = cookie.Id, UtcTimestamp = _clockService.Now };
        //     var activeVisit = await _visitService.GetByActivityAsync(pageView, token).ConfigureAwait(false);

        //     IActionResult result = NotFound();
        //     if(activeVisit != null)
        //     {
        //         result = Ok(activeVisit);
        //     } // end if

        //     return result;
        // } // end method

        // [HttpGet("unidentified")]
        // public async Task<IActionResult> GetUnidentifiedAsync(
        //     [FromCookie(Name = CookieConstants.NAME)] string? cookieValue = null,
        //     [FromQuery(Name = "cookieId")] Guid? cookieId = null,
        //     CancellationToken token = default)
        // {
        //     var now = _clockService.Now;
        //     var visitExpiration = await _visitService.GetVisitExpirationAsync(now, token).ConfigureAwait(false);
        //     var cookie = cookieValue.ToCookieModel(visitExpiration);
        //     if(cookieId.HasValue)
        //     {
        //         cookie.Id = cookieId.Value;
        //     } // end if

        //     var unidentifiedVisits = await _visitService.GetUnidentifiedAsync(cookie.Id, now, null, token);

        //     IActionResult result = NotFound();
        //     if(unidentifiedVisits != null && unidentifiedVisits.Data != null && unidentifiedVisits.Data.Any())
        //     {
        //         result = Ok(unidentifiedVisits);
        //     } // end if

        //     return result;
        // } // end method

        // [HttpPost]
        // public async Task<IActionResult> CreateVisitAsync(
        //     [FromBody] Visit visit,
        //     CancellationToken token = default)
        // {
        //     await _visitService.UpsertAsync(visit, token).ConfigureAwait(false);

        //     return Ok();
        // } // end method

        // [HttpPost("identified")]
        // public async Task<IActionResult> GetIdentifiedAsync(
        //     [FromBody] TimeRange timeRange,
        //     CancellationToken token = default)
        // {
        //     IActionResult result = NotFound();
        //     var response = await _visitService.GetIdentifiedAsync(timeRange, null, token).ConfigureAwait(false);

        //     if(response != null && response.Data != null && response.Data.Any())
        //     {
        //         result = Ok(response);
        //     } // end if

        //     return result;
        // } // end method
    } // end class
} // end namespace