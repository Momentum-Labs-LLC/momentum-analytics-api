using Momentum.Analytics.Core.PII.Models;
using NodaTime;

namespace Momentum.Analytics.Lambda.Api.Cookies
{
    public class Cookie
    {
        public Guid Id { get; set; }
        public Ulid VisitId { get; set; }
        public Instant VisitExpiration { get; set; }
        public PiiTypeEnum CollectedPii { get; set; }
        public int MaxFunnelStep { get; set; }
        public string? UserId { get; set; }
    } // end class
} // end namespace