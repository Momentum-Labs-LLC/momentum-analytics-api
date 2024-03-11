using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.Core.Visits.Models;

namespace Momentum.Analytics.Core.Visits.Interfaces
{
    public interface IVisitService
    {
        Task<Visit> GetAsync(Guid id, CancellationToken token = default);
        Task<Visit> GetByActivityAsync(IUserActivity userActivity, CancellationToken token = default);
        Task UpsertAsync(Visit visit, CancellationToken token = default);
        // Task CreateVisitAsync(Visit visit, CancellationToken token = default);
        // Task<Visit> ExtendVisitAsync(Guid id, IUserActivity userActivity, CancellationToken token = default);
        // Task IdentifyVisitAsync(Guid id, PiiValue piiValue, CancellationToken token = default);
        // Task UpdatePiiAsync(Guid id, PiiValue piiValue, CancellationToken token = default);
        // Task UpdateFunnelStepAsync(Guid id, int funnelStep, CancellationToken token = default);
        Task<ISearchResponse<Visit>> SearchAsync(IVisitSearchRequest searchRequest, CancellationToken token = default);
    } // end interface
} // end namespace