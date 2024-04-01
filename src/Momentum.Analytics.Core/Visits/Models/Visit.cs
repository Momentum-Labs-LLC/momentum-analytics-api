using Momentum.Analytics.Core.PII.Models;
using NodaTime;

namespace Momentum.Analytics.Core.Visits.Models
{
    public class Visit
    {
        /// <summary>
        /// Gets or sets the identifier of the visit itself.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the cookie identifier.
        /// </summary>
        public Guid CookieId { get; set; }

        /// <summary>
        /// Gets or sets the start of the visit.
        /// </summary>
        public Instant UtcStart { get; set; }

        /// <summary>
        /// Gets or sets the expected expiration of the visit.
        /// This could be extended, depending on the definition of a visit.
        /// </summary>
        public Instant? UtcExpiration { get; set; }

        /// <summary>
        /// Gets or sets the conversion funnel step.
        /// </summary>
        public int FunnelStep { get; set; } = 0;

        /// <summary>
        /// Gets or sets the referrer domain
        /// </summary>
        public string? Referer { get; set; }

        /// <summary>
        /// Gets or sets the utm_source value of the visit.
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// Gets or sets the utm_medium value of the visit.
        /// </summary>
        public string? Medium { get; set; }

        /// <summary>
        /// Gets or sets the pii value.
        /// </summary>
        public string? PiiValue { get; set; }

        /// <summary>
        /// Gets or sets the pii type id.
        /// </summary>
        public PiiTypeEnum? PiiType { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the identification of the visit.
        /// </summary>
        public Instant? UtcIdentifiedTimestamp { get; set; }
    } // end class
} // end namespace