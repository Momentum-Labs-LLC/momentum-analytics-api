using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.Core.Visits.Interfaces;

namespace Momentum.Analytics.Core.Visits
{
    public class IdentifiedVisitSearchRequest : IIdentifiedVisitSearchRequest
    {
        public string PiiValue { get; set; }

        public PiiTypeEnum PiiType { get; set; }

        public DateTime UtcStartWindow { get; set; }

        public DateTime? UtcEndWindow { get; set; }
    } // end class
} // end namespace