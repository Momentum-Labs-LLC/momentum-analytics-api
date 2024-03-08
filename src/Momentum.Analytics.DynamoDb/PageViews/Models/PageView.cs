namespace Momentum.Analytics.DynamoDb.PageViews.Models
{
    public class PageView
    {
        /// <summary>
        /// Gets or sets the request id.
        /// </summary>
        public string RequestId { get; set; }

        /// <summary>
        /// Gets or sets the cookie id.
        /// </summary>
        public Guid CookieId { get; set; }

        /// <summary>
        /// Gets or sets the timestamp in milliseconds since epoch.
        /// </summary>
        public long UtcTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the domain of the page being viewed.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the path of the page being viewed.
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// Gets or sets the referrer for the page being viewed.
        /// </summary>
        public string? Referrer { get; set; }

        /// <summary>
        /// Gets or sets the utm_source value.
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// Gets or sets the utm_medium value.
        /// </summary>
        public string? Medium { get; set; }
    } // end class
} // end namespace