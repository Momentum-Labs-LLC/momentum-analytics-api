namespace Momentum.Analytics.Lambda.Api.Cookies
{
    public class CookieOptionsFactory : ICookieOptionsFactory
    {
        protected readonly TimeSpan _expiration;

        public readonly string _domain;

        public readonly string? _path;

        public readonly bool _isSecure;

        public readonly SameSiteMode _sameSiteMode;

        public CookieOptionsFactory(IConfiguration configuration)
        {
            var expDays = configuration.GetValue<int>(CookieConstants.COOKIE_EXPIRATION, CookieConstants.COOKIE_EXP_DAYS_DEFAULT);
            if(expDays > 0)
            {
                _expiration = TimeSpan.FromDays(expDays);
            } 
            else
            {
                _expiration = CookieConstants.COOKIE_EXPIRATION_DEFAULT;
            } // end if

            _domain = configuration.GetValue<string>(CookieConstants.COOKIE_DOMAIN, CookieConstants.COOKIE_DOMAIN_DEFAULT);
            _path = configuration.GetValue<string>(CookieConstants.COOKIE_PATH, CookieConstants.COOKIE_PATH_DEFAULT);
            _isSecure = configuration.GetValue<bool>(CookieConstants.COOKIE_SECURE, CookieConstants.COOKIE_SECURE_DEFAULT);
            var sameSiteValue = configuration.GetValue<int>(CookieConstants.COOKIE_SAME_SITE_MODE, (int)CookieConstants.COOKIE_SAME_SITE_MODE_DEFAULT);

            _sameSiteMode = (SameSiteMode)sameSiteValue;
        } // end method

        public virtual async Task<CookieOptions> GetAsync(CancellationToken token = default)
        {
            return new CookieOptions()
                {
                    Expires = DateTime.UtcNow.Add(_expiration),
                    Domain = _domain,
                    Path = _path,
                    SameSite = _sameSiteMode, // Default
                    Secure = _isSecure,
                    HttpOnly = false, // let javascript access the cookie
                };      
        } // end method
    } // end class
} // end namespace