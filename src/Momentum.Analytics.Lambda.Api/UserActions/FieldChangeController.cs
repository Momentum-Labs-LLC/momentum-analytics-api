using System.Text.Json;
using FluentResults;
using FluentResults.Extensions;
using Microsoft.AspNetCore.Mvc;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.UserActions;
using Momentum.Analytics.Core.UserActions.Interfaces;
using Momentum.Analytics.Lambda.Api.Cookies;
using Momentum.FromCookie;

namespace Momentum.Analytics.Lambda.Api.UserActions;

[ApiController]
[Route("v1/field-changes")]
public class FieldChangeController : ControllerBase
{
    protected readonly IFieldChangeService _fieldChangeService;
    protected readonly IHttpContextAccessor _httpContextAccessor;
    protected readonly ICookieWriter _cookieWriter;
    protected readonly IClockService _clockService;
    protected readonly IVisitWindowCalculator _visitWindowCalculator;
    protected readonly ILogger _logger;

    public FieldChangeController(
        IFieldChangeService fieldChangeService,
        IHttpContextAccessor httpContextAccessor,
        ICookieWriter cookieWriter,
        IClockService clockService,
        IVisitWindowCalculator visitWindowCalculator,
        ILogger<FieldChangeController> logger)
    {
        _fieldChangeService = fieldChangeService ?? throw new ArgumentNullException(nameof(fieldChangeService));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _cookieWriter = cookieWriter ?? throw new ArgumentNullException(nameof(cookieWriter));
        _clockService = clockService ?? throw new ArgumentNullException(nameof(clockService));
        _visitWindowCalculator = visitWindowCalculator ?? throw new ArgumentNullException(nameof(visitWindowCalculator));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    } // end method

    [HttpGet(ApiConstants.PIXEL_PATH)]
    public async Task<IActionResult> PiiPixelAsync(
        [FromQuery(Name = ApiConstants.QUERY_STRING_EVENT_KEY)] string base64Pii,
        [FromCookie(Name = CookieConstants.NAME)] string? cookieValue = null,
        CancellationToken token = default)
    {
        var procesingResult = await Result.Ok(Convert.FromBase64String(base64Pii))
            .Bind(bytes => Result.Try(() => new MemoryStream(bytes)))
            .Bind(stream => {
                return Result.Try(() => JsonSerializer.DeserializeAsync<FieldChangeViewModel>(stream, cancellationToken: token))
                    .Bind(viewModel => {
                        if(viewModel == null)
                        {
                            return Result.Fail<FieldChangeViewModel>("Failed to deserialize field change view model.");
                        }
                        return Result.Ok(viewModel);
                    });
            })
            .Bind(async viewModel => await AcceptFieldChangeAsync(viewModel, cookieValue, token));

        if(procesingResult.IsFailed)
        {
            _logger.LogError(new EventId(0), "Failed to accept field change. {Errors}", string.Join(", ", procesingResult.Errors.Select(error => error.Message)));
            return StatusCode(500);
        } // end if

        return File(ApiConstants.GIF_BYTES, "image/gif");;
    } // end method

    [HttpPost]
    public async Task<IActionResult> RecordAsync(
        [FromBody] FieldChangeViewModel viewModel,
        [FromCookie(Name = CookieConstants.NAME)] string? cookieValue = null,
        CancellationToken token = default)
    {
        var result = await AcceptFieldChangeAsync(viewModel, cookieValue, token).ConfigureAwait(false);

        if(result.IsFailed)
        {
            _logger.LogError(new EventId(0), "Failed to accept field change. {Errors}", string.Join(", ", result.Errors.Select(error => error.Message)));
            return StatusCode(500);
        } // end if

        return Ok();
    } // end method

    protected async Task<Result> AcceptFieldChangeAsync(
        FieldChangeViewModel viewModel,
        string? cookieValue = null,
        CancellationToken token = default)
    {
        var now = _clockService.Now;
        var visitExpiration = await _visitWindowCalculator.GetExpirationAsync(now, token).ConfigureAwait(false);

        var referrer = _httpContextAccessor.HttpContext?.Request.Headers["Referer"].ToString() ?? string.Empty;
        var cookie = cookieValue.ToCookieModel(visitExpiration)
            .UpdateVisitFields(now, visitExpiration);        

        var domainModel = new FieldChange()
        {
            CookieId = cookie.Id,
            VisitId = cookie.VisitId,
            UtcTimestamp = now,
            Url = referrer,
            Field = viewModel.Field,
            Value = viewModel.Value
        };

        return await Result
            .Ok(_fieldChangeService.RecordAsync(domainModel, token))
            .Bind(_ => Result.Try(() => _cookieWriter.SetCookieAsync(cookie, token)));
    } // end method
} // end class