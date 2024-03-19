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

        public static TimeSpan COOKIE_EXPIRATION = TimeSpan.FromDays(30);
    } // end class
} // end namespace