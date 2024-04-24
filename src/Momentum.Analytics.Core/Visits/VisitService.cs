using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using NodaTime;

namespace Momentum.Analytics.Core.Visits
{
    public class VisitService<TPage, TSearchResponse, TStorage> : 
            IVisitService<TPage, TSearchResponse>
        where TSearchResponse : ISearchResponse<Visit, TPage>
        where TStorage : IVisitStorage<TPage, TSearchResponse>
    {
        protected readonly IVisitWindowCalculator _visitWindowCalculator;
        protected readonly TStorage _visitStorage;        
        protected readonly ILogger _logger;

        public VisitService(
            IVisitWindowCalculator visitWindowCalculator,
            TStorage visitStorage,
            ILogger<VisitService<TPage, TSearchResponse, TStorage>> logger)
        {
            _visitWindowCalculator = visitWindowCalculator ?? throw new ArgumentNullException(nameof(visitWindowCalculator));
            _visitStorage = visitStorage ?? throw new ArgumentNullException(nameof(visitStorage));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        public virtual async Task<Instant> GetVisitExpirationAsync(Instant activityTimestamp, CancellationToken token = default)
        {
            return await _visitWindowCalculator.GetExpirationAsync(activityTimestamp, token).ConfigureAwait(false);
        } // end method

        public virtual async Task<Visit?> GetByActivityAsync(IUserActivity userActivity, CancellationToken token = default)
        {
            Visit? result = null;
            
            var latestVisit = await _visitStorage.GetLatestAysnc(userActivity.CookieId, userActivity.UtcTimestamp, token).ConfigureAwait(false);
            if(latestVisit != null 
                && latestVisit.UtcStart <= userActivity.UtcTimestamp
                && latestVisit.UtcExpiration > userActivity.UtcTimestamp)
            {
                result = latestVisit;
            } // end if

            return result;
        } // end method

        public virtual async Task<TSearchResponse> GetIdentifiedAsync(ITimeRange timeRange, TPage page, CancellationToken token = default)
        {
            return await _visitStorage.GetIdentifiedAsync(timeRange, page, token).ConfigureAwait(false);
        } // end method

        public virtual async Task<TSearchResponse> GetUnidentifiedAsync(Guid cookieId, Instant timestamp, TPage page, CancellationToken token = default)
        {
            return await _visitStorage.GetUnidentifiedAsync(cookieId, timestamp, page, token).ConfigureAwait(false);
        } // end method

        public virtual async Task<TSearchResponse> GetUnidentifiedAsync(ITimeRange timeRange, TPage page, CancellationToken token = default)
        {
            return await _visitStorage.GetUnidentifiedAsync(timeRange, page, token).ConfigureAwait(false);
        } // end method

        public virtual async Task UpsertAsync(Visit visit, CancellationToken token = default)
        {
            await _visitStorage.UpsertAsync(visit, token).ConfigureAwait(false);
        } // end method
    } // end class
} // end namespace