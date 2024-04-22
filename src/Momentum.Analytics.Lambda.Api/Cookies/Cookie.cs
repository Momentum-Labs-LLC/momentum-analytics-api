using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Momentum.Analytics.Core.PII.Models;
using NodaTime;

namespace Momentum.Analytics.Lambda.Api.Cookies
{
    public class Cookie
    {
        public Guid Id { get; set; }
        public Instant VisitExpiration { get; set; }
        public PiiTypeEnum CollectedPii { get; set; }
        public int MaxFunnelStep { get; set; }
        public string? UserId { get; set; }
    } // end class
} // end namespace