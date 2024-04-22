using NodaTime;

namespace Momentum.Analytics.Core.Interfaces
{
    public interface IVisitWindowCalculator
    {
        Task<Instant> GetExpirationAsync(Instant activityTimestamp, CancellationToken token = default);
        //Task<Instant> GetPotentialStartAsync(Instant activityTimestamp, CancellationToken token = default);
    } // end interface
} // end namespace