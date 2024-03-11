using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Models;
using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.Core.Visits.Interfaces;

namespace Momentum.Analytics.Core.Visits
{
    public class VisitSearchRequest : SearchRequest, IVisitSearchRequest
    {
        public Guid? CookieId { get; set; }

        public DateTime? UtcActivityTimestamp { get; set; }
        public bool? IsIdentified { get; set; }

        public string? PiiValue { get; set; }

        public PiiTypeEnum? PiiType { get; set; }

        public ITimeRange? IdentifiedTimeRange { get; set; }
    } // end class
} // end namespace