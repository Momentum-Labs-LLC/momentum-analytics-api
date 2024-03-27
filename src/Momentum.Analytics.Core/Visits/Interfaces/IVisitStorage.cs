using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using NodaTime;

namespace Momentum.Analytics.Core.Visits.Interfaces
{
    public interface IVisitStorage<TPage, TSearchResponse>
        where TSearchResponse : ISearchResponse<Visit, TPage>
    {        
        Task<Visit?> GetAsync(Guid id, CancellationToken token = default);
        Task UpsertAsync(Visit visit, CancellationToken token = default);
        Task DeleteAsync(Guid id, CancellationToken token = default);
        Task<Visit?> GetByActivityAsync(Guid cookieId, ITimeRange expirationTimeRange, CancellationToken token = default);
        Task<TSearchResponse> GetIdentifiedAsync(ITimeRange timeRange, TPage page, CancellationToken token = default);
        Task<TSearchResponse> GetUnidentifiedAsync(Guid cookieId, TPage page, CancellationToken token = default);
    } // end interface
} // end namespace