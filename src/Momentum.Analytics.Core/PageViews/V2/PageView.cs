using Momentum.Analytics.Core.Models;

namespace Momentum.Analytics.Core.PageViews.V2
{
    public class PageView : UserActivity
    {
        /// <summary>
        /// Gets or sets the full url of the page being viewed.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the referrer of the page being viewed.
        /// </summary>
        public string? Referer { get; set; }
    } // end class
} // end namespace