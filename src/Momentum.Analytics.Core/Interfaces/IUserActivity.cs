using NodaTime;

namespace Momentum.Analytics.Core.Interfaces
{
    public interface IUserActivity
    {
        Guid CookieId { get; }
        Instant UtcTimestamp { get; }
    } // end interface
} // end namespace