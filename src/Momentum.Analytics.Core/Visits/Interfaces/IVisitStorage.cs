using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Visits.Models;

namespace Momentum.Analytics.Core.Visits.Interfaces
{
    public interface IVisitStorage
    {
        Task<ISearchResponse<Visit>> SearchAsync(IVisitSearchRequest request, CancellationToken token = default);
        Task<Visit?> GetAsync(Guid id, CancellationToken token = default);
        Task UpsertAsync(Visit visit, CancellationToken token = default);
        Task DeleteAsync(Guid id, CancellationToken token = default);
    } // end interface
} // end namespace