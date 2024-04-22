using Momentum.Analytics.Core.Interfaces;

namespace Momentum.Analytics.Visits.Lambda.IdentifiedVisits.Interfaces
{
    public interface IS3OutputConfiguration
    {
        string Bucket { get; }

        Task<string> BuildKeyAsync(ITimeRange timeRange, CancellationToken token = default);
    } // end namespace
} // end namespace