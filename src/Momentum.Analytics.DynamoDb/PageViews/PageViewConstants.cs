namespace Momentum.Analytics.DynamoDb.PageViews
{
    public static class PageViewConstants
    {
        public const string TABLE_NAME_KEY = "PAGE_VIEWS_TABLE";
        public const string TABLE_NAME_DEFAULT = "page_views";

        public const string REQUEST_ID = "RequestId";
        public const string COOKIE_ID = "CookieId";
        public const string UTC_TIMESTAMP = "UtcTimestamp";
        public const string DOMAIN = "Domain";
        public const string PATH = "Path";
        public const string FUNNEL_STEP = "FunnelStep";
        public const string REFERER = "Referer";
        public const string SOURCE = "Source";
        public const string MEDIUM = "Medium"; 
    } // end class
} // end namespace