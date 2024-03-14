using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Core.Visits.Models;

namespace Momentum.Analytics.Core.Visits
{
    public class VisitService<TPage, TSearchResponse, TStorage> : 
            IVisitService<TPage, TSearchResponse>
        where TSearchResponse : ISearchResponse<Visit, TPage>
        where TStorage : IVisitStorage<TPage, TSearchResponse>
    {
        protected readonly TStorage _visitStorage;
        protected readonly IMemoryCache _memoryCache;
        protected readonly ILogger _logger;

        public VisitService(
            TStorage visitStorage,
            IMemoryCache memoryCache,
            ILogger<VisitService<TPage, TSearchResponse, TStorage>> logger)
        {
            _visitStorage = visitStorage ?? throw new ArgumentNullException(nameof(visitStorage));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        public virtual async Task<Visit?> GetAsync(Guid id, CancellationToken token = default)
        {
            var result = await GetVisitFromCacheAsync(id, token).ConfigureAwait(false);
            if(result == null)
            {
                result = await _visitStorage.GetAsync(id, token).ConfigureAwait(false);

                if(result != null)
                {
                    await SetVisitInCacheAsync(result, token).ConfigureAwait(false);
                } // end if
            } // end if

            return result;
        } // end method

        public virtual async Task<Visit?> GetByActivityAsync(IUserActivity userActivity,CancellationToken token = default)
        {
            Visit? result = await _visitStorage.GetByActivityAsync(userActivity.CookieId, userActivity.UtcTimestamp, token).ConfigureAwait(false);

            if(result != null)
            {
                await SetVisitInCacheAsync(result, token).ConfigureAwait(false);
            } // end if

            return result;
        } // end method

        public async Task<TSearchResponse> GetIdentifiedAsync(ITimeRange timeRange, TPage page, CancellationToken token = default)
        {
            return await _visitStorage.GetIdentifiedAsync(timeRange, page, token).ConfigureAwait(false);
        } // end method

        public async Task<TSearchResponse> GetUnidentifiedAsync(Guid cookieId, TPage page, CancellationToken token = default)
        {
            return await _visitStorage.GetUnidentifiedAsync(cookieId, page, token).ConfigureAwait(false);
        } // end method

        public virtual async Task UpsertAsync(Visit visit, CancellationToken token = default)
        {
            await _visitStorage.UpsertAsync(visit, token).ConfigureAwait(false);
            await SetVisitInCacheAsync(visit, token).ConfigureAwait(false);
        } // end method
        
        protected string BuildKey(Guid visitId)
        {
            return $"VISIT:{visitId.ToString()}";
        } // end method

        protected virtual async Task<Visit?> GetVisitFromCacheAsync(Guid visitId, CancellationToken token = default)
        {
            Visit? result = null;
            var cacheKey = BuildKey(visitId);
            if(_memoryCache.TryGetValue(cacheKey, out Visit? tmpResult)
                && result != null)
            {
                result = tmpResult;
            } // end method

            return result;
        } // end method

        protected virtual async Task SetVisitInCacheAsync(Visit visit, CancellationToken token = default)
        {
            var cacheKey = BuildKey(visit.Id);
            _memoryCache.Set(cacheKey, visit, TimeSpan.FromSeconds(10));
        } // end method
    } // end class
} // end namespace