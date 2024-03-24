using Momentum.Analytics.Core.Interfaces;

namespace Momentum.Analytics.Visits.Lambda
{
    public interface IS3OutputConfiguration
    {
        string Bucket { get; }

        Task<string> BuildKeyAsync(ITimeRange timeRange, CancellationToken token = default);
    } // end namespace
} // end namespace