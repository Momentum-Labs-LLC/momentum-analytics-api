using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.Lambda.Api.Cookies;
using Momentum.Analytics.Lambda.Api.Pii.ViewModels;
using Momentum.FromCookie;

namespace Momentum.Analytics.Lambda.Api.Pii
{
    [ApiController]
    [Route("v1/pii")]
    public class PiiController : ControllerBase
    {
        protected readonly IPiiService _piiService;
        protected readonly ICookieWriter _cookieWriter;
        protected readonly IClockService _clockService;
        protected readonly IVisitExpirationProvider _visitExpirationProvider;
        protected readonly ILogger _logger;

        public PiiController(
            IPiiService piiService, 
            ICookieWriter cookieWriter,
            IClockService clockService,
            IVisitExpirationProvider visitExpirationProvider,
            ILogger<PiiController> logger)
        {
            _piiService = piiService ?? throw new ArgumentNullException(nameof(piiService));
            _cookieWriter = cookieWriter ?? throw new ArgumentNullException(nameof(cookieWriter));
            _clockService = clockService ?? throw new ArgumentNullException(nameof(clockService));
            _visitExpirationProvider = visitExpirationProvider ?? throw new ArgumentNullException(nameof(visitExpirationProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        [HttpGet(ApiConstants.PIXEL_PATH)]
        public async Task<IActionResult> PiiPixelAsync([FromQuery(Name = ApiConstants.QUERY_STRING_EVENT_KEY)] string base64PageView,
            [FromCookie(Name = CookieConstants.NAME)] string? cookieValue = null,
            CancellationToken token = default)
        {
            IActionResult result = File(ApiConstants.GIF_BYTES, "image/gif");

            PiiViewModel viewModel;
            var pageViewBytes = Convert.FromBase64String(base64PageView);
            using (var stream = new MemoryStream(pageViewBytes))
            {
                viewModel = await JsonSerializer.DeserializeAsync<PiiViewModel>(stream, cancellationToken: token).ConfigureAwait(false);
            } // end using

            if(viewModel != null)
            {
                try
                {
                    await AcceptPiiAsync(viewModel, cookieValue, token).ConfigureAwait(false);
                }
                catch(Exception ex)
                {
                    _logger.LogError(new EventId(0), ex, "Failed to accept pii.");
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
        public async Task<IActionResult> RecordAsync(
            [FromBody] PiiViewModel piiViewModel,
            [FromCookie(Name = CookieConstants.NAME)] string? cookieValue = null,
            CancellationToken token = default)
        {
            IActionResult result = Ok();
            try
            {
                await AcceptPiiAsync(piiViewModel, cookieValue, token).ConfigureAwait(false);
            }
            catch(Exception ex)
            {
                _logger.LogError(new EventId(0), ex, "Failed to accept pii");
                result = StatusCode(500);
            } // end try/catch
            
            return result;
        } // end method

        [HttpPost("builder")]
        public async Task<IActionResult> BuildPixelAsync(
            [FromBody] PiiViewModel piiViewModel,
            CancellationToken token = default)
        {
             var json = JsonSerializer.Serialize(piiViewModel);

            var base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));

            return Ok(base64String);
        } // end method

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAsync(
            [FromRoute] Guid id,
            CancellationToken token = default)
        {
            IActionResult result = NotFound();

            var pii = await _piiService.GetPiiAsync(id, token).ConfigureAwait(false);
            if(pii != null)
            {
                result = Ok(pii);
            } // end if

            return result;
        } // end method

        [HttpGet]
        public async Task<IActionResult> GetByCookieAsync(
            [FromCookie(Name = CookieConstants.NAME)] string? cookieValue = null,
            [FromQuery(Name = "cookieId")] Guid? cookieId = null,
            CancellationToken token = default)
        {
            IActionResult result = NotFound();

            var now = _clockService.Now;
            var visitExpiration = await _visitExpirationProvider.GetExpirationAsync(now, token).ConfigureAwait(false);
            var cookie = cookieValue.ToCookieModel(visitExpiration);
            if(cookieId.HasValue)
            {
                cookie.Id = cookieId.Value;
            }

            var cookiePiis = await _piiService.GetByCookieIdAsync(cookie.Id, token).ConfigureAwait(false);
            if(cookiePiis != null && cookiePiis.Any())
            {
                result = Ok(cookiePiis);
            } // end if

            return result;
        } // end method
    
        protected async Task AcceptPiiAsync(
            PiiViewModel piiViewModel, 
            string? cookieValue = null, 
            CancellationToken token = default)
        {
            var now = _clockService.Now;
            var visitExpiration = await _visitExpirationProvider.GetExpirationAsync(now, token).ConfigureAwait(false);
            var cookie = cookieValue.ToCookieModel(visitExpiration);

            var collectedPii = new CollectedPii()
            {
                CookieId = cookie.Id,
                UtcTimestamp = _clockService.Now,
                Pii = new PiiValue()
                {
                    Value = piiViewModel.Value,
                    PiiType = piiViewModel.Type
                }
            };

            await _piiService.RecordAsync(collectedPii, token).ConfigureAwait(false);

            if(!cookie.CollectedPii.HasFlag(piiViewModel.Type))
            {
                cookie.CollectedPii = cookie.CollectedPii | piiViewModel.Type;
            } // end if

            await _cookieWriter.SetCookieAsync(cookie, token).ConfigureAwait(false);
        } // end method
    } // end class
} // end namespace