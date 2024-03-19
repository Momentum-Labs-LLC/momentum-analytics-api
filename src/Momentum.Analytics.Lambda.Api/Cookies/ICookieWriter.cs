namespace Momentum.Analytics.Lambda.Api.Cookies
{
    public interface ICookieWriter
    {
        Task SetCookieAsync(Cookie cookie, CancellationToken token = default);
    } // end interface

    public class CookieWriter : ICookieWriter
    {
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly ILogger _logger;

        public CookieWriter(IHttpContextAccessor httpContextAccessor, ILogger<CookieWriter> logger)
        {
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        public Task SetCookieAsync(Cookie cookie, CancellationToken token = default)
        {
            var cookieOptions = new CookieOptions()
                {
                    Expires = DateTime.UtcNow.Add(CookieConstants.COOKIE_EXPIRATION),
                    Domain = BuildCookieDomain(),
                    Path = "/",
                    SameSite = SameSiteMode.Lax, // Default
                    Secure = false,
                    HttpOnly = false,
                };
            
            _httpContextAccessor.HttpContext.Response.Cookies.Append(CookieConstants.NAME, cookie.ToHeaderValue(), cookieOptions);

            return Task.CompletedTask;
        } // end method

        protected string BuildCookieDomain()
        {
            return _httpContextAccessor.HttpContext.Request.Host.Host;
        } // end method
    } // end class
} // end namespace