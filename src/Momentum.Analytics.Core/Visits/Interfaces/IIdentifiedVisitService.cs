using Momentum.Analytics.Core.PageViews.Models;
using Momentum.Analytics.Core.PII.Models;

namespace Momentum.Analytics.Core.Visits.Interfaces
{
    public interface IIdentifiedVisitService
    {
        Task CreateVisitAsync(PiiValue piiValue, IEnumerable<PageView> pageViews, CancellationToken token = default);
        Task UpdatePiiAsync(IdentifiedVisit visit, PiiValue piiValue, CancellationToken token = default);
        Task UpdateFunnelStepAsync(IdentifiedVisit visit, int funnelStep, CancellationToken token = default);
        Task<IdentifiedVisit?> GetActiveVisitAsync(PiiValue piiValue, CancellationToken token = default);
        Task<IEnumerable<IdentifiedVisit>> SearchAsync(IIdentifiedVisitSearchRequest searchRequest, CancellationToken token = default);
    } // end interface
} // end namespace