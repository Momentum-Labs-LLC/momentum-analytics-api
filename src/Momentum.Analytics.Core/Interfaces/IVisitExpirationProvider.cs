using NodaTime;

namespace Momentum.Analytics.Core.Interfaces
{
    public interface IVisitExpirationProvider
    {
        Task<Instant> GetExpirationAsync(Instant activityTimestamp, CancellationToken token = default);
    } // end interface
} // end namespace