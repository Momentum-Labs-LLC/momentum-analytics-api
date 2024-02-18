using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Momentum.Pixel.DynamoDb.PageViews.Models
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
        /// Gets or sets the timestamp.
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
        public UrchinTrackingParameters? UtmParameters { get; set; }
    } // end class
} // end namespace