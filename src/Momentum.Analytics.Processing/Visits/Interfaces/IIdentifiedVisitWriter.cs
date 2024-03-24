using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Visits.Models;

namespace Momentum.Analytics.Processing.Visits.Interfaces
{
    public interface IIdentifiedVisitWriter
    {
        Task WriteAsync(ITimeRange timeRange, IEnumerable<Visit> visits, CancellationToken token = default);
    } // end interface
} // end namespace