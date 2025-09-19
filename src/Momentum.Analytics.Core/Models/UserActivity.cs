using Momentum.Analytics.Core.Interfaces;
using NodaTime;

namespace Momentum.Analytics.Core.Models
{
    public class UserActivity : IUserActivity
    {
        public Guid CookieId { get; set; }
        public Ulid VisitId { get; set; }
        public Instant UtcTimestamp { get; set; }
    } // end class
} // end namespace