using Microsoft.AspNetCore.Mvc;
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
        protected readonly ILogger _logger;

        public PiiController(
            IPiiService piiService, 
            ICookieWriter cookieWriter,
            ILogger<PiiController> logger)
        {
            _piiService = piiService ?? throw new ArgumentNullException(nameof(piiService));
            _cookieWriter = cookieWriter ?? throw new ArgumentNullException(nameof(cookieWriter));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        [HttpPost]
        public async Task<IActionResult> RecordAsync(
            [FromBody] PiiViewModel piiViewModel,
            [FromCookie(Name = CookieConstants.NAME)] string? cookieValue = null,
            CancellationToken token = default)
        {
            var cookie = cookieValue.ToCookieModel();

            var collectedPii = new CollectedPii()
            {
                CookieId = cookie.Id,
                UtcTimestamp = DateTime.UtcNow,
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

            return Ok();            
        } // end method
    } // end class
} // end namespace