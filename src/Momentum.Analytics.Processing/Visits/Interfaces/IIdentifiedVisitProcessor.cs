using Momentum.Analytics.Core.Interfaces;

namespace Momentum.Analytics.Processing.Visits.Interfaces
{
    public interface IIdentifiedVisitProcessor
    {
        Task ExportAsync(ITimeRange timeRange, CancellationToken token = default);
    } // end interface
} // end namespace