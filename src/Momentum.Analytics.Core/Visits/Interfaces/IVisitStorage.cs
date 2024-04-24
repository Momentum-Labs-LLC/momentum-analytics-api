using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using NodaTime;

namespace Momentum.Analytics.Core.Visits.Interfaces
{
    public interface IVisitStorage<TPage, TSearchResponse>
        where TSearchResponse : ISearchResponse<Visit, TPage>
    {
        Task UpsertAsync(Visit visit, CancellationToken token = default);
        Task<Visit?> GetLatestAysnc(Guid cookieId, Instant timestamp, CancellationToken token = default);
        Task<TSearchResponse> GetIdentifiedAsync(ITimeRange timeRange, TPage page, CancellationToken token = default);
        Task<TSearchResponse> GetUnidentifiedAsync(ITimeRange timeRange, TPage page, CancellationToken token = default);
        Task<TSearchResponse> GetUnidentifiedAsync(Guid cookieId, Instant timestamp, TPage page, CancellationToken token = default);
    } // end interface
} // end namespace