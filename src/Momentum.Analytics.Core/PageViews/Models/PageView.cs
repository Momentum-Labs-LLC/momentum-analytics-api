namespace Momentum.Analytics.Core.PageViews.Models
{
    /// <summary>
    /// A class representing a users viewing of a specific web page.
    /// </summary>
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
        /// Gets or sets the timestamp.
        /// </summary>
        public DateTime UtcTimestamp { get; set; }

        /// <summary>
        /// Gets or sets the domain of the page being viewed.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// Gets or sets the path of the page being viewed.
        /// </summary>
        public string? Path { get; set; }

        /// <summary>
        /// Gets or sets the funnel step.
        /// </summary>
        public int FunnelStep { get; set; } = 0;

        /// <summary>
        /// Gets or sets the query string of the page being viewed.
        /// </summary>
        public Dictionary<string, string>? QueryString { get; set; }

        /// <summary>
        /// Gets or sets the referrer for the page being viewed.
        /// </summary>
        public string? ReferrerDomain { get; set; }

        /// <summary>
        /// Gets or sets teh urchin tracking parameters.
        /// </summary>
        public IEnumerable<UrchinTrackingParameter>? UtmParameters { get; set; }
    } // end class
} // end namespace