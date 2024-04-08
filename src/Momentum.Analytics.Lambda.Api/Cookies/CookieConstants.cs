using Microsoft.AspNetCore.Cors.Infrastructure;

namespace Momentum.Analytics.Lambda.Api.Cookies
{
    public static class CookieConstants
    {
        public const string NAME = "mllc";
        public const string COOKIE_ID = "id";
        public const string VISIT_EXPIRATION = "vexp";
        public const string PII_BITMAP = "pii";
        public const string MAX_FUNNEL_STEP = "fs";
        public const string COOKIE_VALUE_DELIMITER = "=";


        public const string COOKIE_EXPIRATION = "COOKIE_EXP";
        public const int COOKIE_EXP_DAYS_DEFAULT = 365;
        public static TimeSpan COOKIE_EXPIRATION_DEFAULT = TimeSpan.FromDays(365);
        public const string COOKIE_DOMAIN = "COOKIE_DOMAIN";
        public const string COOKIE_DOMAIN_DEFAULT = "localhost";
        public const string COOKIE_PATH = "COOKIE_PATH";
        public const string COOKIE_PATH_DEFAULT = "/";
        public const string COOKIE_SAME_SITE_MODE = "COOKIE_SSM";
        public static SameSiteMode COOKIE_SAME_SITE_MODE_DEFAULT = SameSiteMode.None;
        public const string COOKIE_SECURE = "COOKIE_SECURE";
        public const bool COOKIE_SECURE_DEFAULT = true;

    } // end class
} // end namespace