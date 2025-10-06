using Microsoft.Extensions.Primitives;
using Momentum.Analytics.Core.Interfaces;

namespace Momentum.Analytics.Lambda.Api.Cookies
{
    public class CookieWriter : ICookieWriter
    {   
        protected readonly ICookieOptionsFactory _cookieOptionsFactory;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        protected readonly ILogger _logger;

        public CookieWriter(
            ICookieOptionsFactory cookieOptionsFactory, 
            IHttpContextAccessor httpContextAccessor,
            ILogger<CookieWriter> logger)
        {
            _cookieOptionsFactory = cookieOptionsFactory ?? throw new ArgumentNullException(nameof(cookieOptionsFactory));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        public async Task SetCookieAsync(Cookie cookie, CancellationToken token = default)
        {
            var cookieOptions = await _cookieOptionsFactory.GetAsync(token).ConfigureAwait(false);   
            
            _httpContextAccessor.HttpContext?.Response.Cookies.Append(CookieConstants.NAME, cookie.ToHeaderValue().ToString(), cookieOptions);
        } // end method
    } // end class
} // end namespace