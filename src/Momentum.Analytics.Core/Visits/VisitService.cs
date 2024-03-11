using System.Data;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Core.Visits.Models;

namespace Momentum.Analytics.Core.Visits
{
    public class VisitService : IVisitService
    {
        protected readonly IVisitConfiguration _visitConfiguration;
        protected readonly IVisitStorage _visitStorage;
        protected readonly IMemoryCache _memoryCache;
        protected readonly ILogger _logger;

        public VisitService(
            IVisitConfiguration visitConfiguration, 
            IVisitStorage visitStorage,
            IMemoryCache memoryCache,
            ILogger<VisitService> logger)
        {
            _visitConfiguration = visitConfiguration ?? throw new ArgumentNullException(nameof(visitConfiguration));
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

        public virtual async Task<Visit?> GetByActivityAsync(IUserActivity userActivity, CancellationToken token = default)
        {
            Visit? result = null;
            var searchRequest = new VisitSearchRequest()
            {
                CookieId = userActivity.CookieId,
                UtcActivityTimestamp = userActivity.UtcTimestamp
            };
            var searchResponse = await _visitStorage.SearchAsync(searchRequest, token).ConfigureAwait(false);

            if(searchResponse != null && searchResponse.Data != null && searchResponse.Data.Any())
            {
                result = searchResponse.Data.First();
                await SetVisitInCacheAsync(result, token).ConfigureAwait(false);
            } // end if

            return result;
        } // end method

        public virtual async Task UpsertAsync(Visit visit, CancellationToken token = default)
        {
            await _visitStorage.UpsertAsync(visit, token).ConfigureAwait(false);
            await SetVisitInCacheAsync(visit, token).ConfigureAwait(false);
        } // end method

        public virtual async Task<ISearchResponse<Visit>> SearchAsync(IVisitSearchRequest searchRequest, CancellationToken token = default)
        {
            return await _visitStorage.SearchAsync(searchRequest, token).ConfigureAwait(false);
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