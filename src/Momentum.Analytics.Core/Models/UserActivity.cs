using Momentum.Analytics.Core.Interfaces;

namespace Momentum.Analytics.Core.Models
{
    public class UserActivity : IUserActivity
    {
        public Guid CookieId { get; set; }

        public DateTime UtcTimestamp { get; set; }
    } // end class
} // end namespace