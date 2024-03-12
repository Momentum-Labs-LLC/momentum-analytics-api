using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Models;
using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.Core.Visits.Interfaces;

namespace Momentum.Analytics.Core.Visits
{
    public class VisitSearchRequest : SearchRequest, IVisitSearchRequest
    {
        /// <summary>
        /// Gets or sets the cookie identifier.
        /// </summary>
        public Guid? CookieId { get; set; }

        /// <summary>
        /// Gets or sets the activity timestamp.
        /// </summary>
        public DateTime? UtcActivityTimestamp { get; set; }

        /// <summary>
        /// Gets or sets if the desired visits are identified or not.
        /// </summary>
        public bool? IsIdentified { get; set; }

        /// <summary>
        /// Gets or sets the desired PII Value.
        /// </summary>
        public string? PiiValue { get; set; }

        /// <summary>
        /// Gets or sets the pii type.
        /// </summary>
        public PiiTypeEnum? PiiType { get; set; }

        /// <summary>
        /// Gets or sets the timerange of when the visits were identified.
        /// </summary>
        public ITimeRange? IdentifiedTimeRange { get; set; }
    } // end class
} // end namespace