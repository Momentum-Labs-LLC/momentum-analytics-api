using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.PII.Models;

namespace Momentum.Analytics.Core.PII.Interfaces
{
    public interface ICollectedPiiStorage<TPage, TSearchResponse>
        where TSearchResponse : ISearchResponse<CollectedPii, TPage>
    {
        Task<IEnumerable<CollectedPii>?> GetByCookieIdAsync(Guid cookieId, CancellationToken token = default);
        Task InsertAysnc(CollectedPii collectedPii, CancellationToken token = default);
        Task<TSearchResponse> GetLatestUserIdsAsync(Guid cookieId, int size = 10, TPage page = default, CancellationToken token = default);
    } // end interface
} // end namespace