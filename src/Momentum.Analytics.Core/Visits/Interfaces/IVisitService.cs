using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using NodaTime;

namespace Momentum.Analytics.Core.Visits.Interfaces
{
    public interface IVisitService<TPage, TSearchResponse>
        where TSearchResponse : ISearchResponse<Visit, TPage>
    {
        Task<Instant> GetVisitExpirationAsync(Instant activityTimestamp, CancellationToken token = default);
        Task<Visit?> GetByActivityAsync(IUserActivity userActivity, CancellationToken token = default);
        Task UpsertAsync(Visit visit, CancellationToken token = default);
        Task<TSearchResponse> GetIdentifiedAsync(ITimeRange timeRange, TPage? page = default, CancellationToken token = default);
        Task<TSearchResponse> GetUnidentifiedAsync(ITimeRange timeRange, TPage? page = default, CancellationToken token = default);
        Task<TSearchResponse> GetUnidentifiedAsync(Guid cookieId, Instant timestamp, TPage? page = default, CancellationToken token = default);
    } // end interface
} // end namespace