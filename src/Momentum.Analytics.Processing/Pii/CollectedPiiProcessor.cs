using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.Processing.Cookies;
using Momentum.Analytics.Processing.Pii.Interfaces;

namespace Momentum.Analytics.Processing.Pii
{
    public class CollectedPiiProcessor<TPage, TVisitSearchResponse, TVisitService> : ICollectedPiiProcessor
        where TVisitSearchResponse : ISearchResponse<Visit, TPage>
        where TVisitService : IVisitService<TPage, TVisitSearchResponse>
    {
        protected readonly TVisitService _visitService;
        protected readonly IClockService _clockService;
        protected readonly IVisitWindowCalculator _visitWindowCalculator;
        protected readonly IPiiService _piiService;
        protected readonly ISharedCookieConfiguration _sharedCookieConfiguration;
        protected readonly ILogger _logger;

        public CollectedPiiProcessor(
            TVisitService visitService,
            IClockService clockService,
            IVisitWindowCalculator visitWindowCalculator,
            IPiiService piiService,
            ISharedCookieConfiguration sharedCookieConfiguration,
            ILogger<CollectedPiiProcessor<TPage, TVisitSearchResponse, TVisitService>> logger)
        {
            _visitService = visitService ?? throw new ArgumentNullException(nameof(visitService));
            _clockService = clockService ?? throw new ArgumentNullException(nameof(clockService));
            _visitWindowCalculator = visitWindowCalculator ?? throw new ArgumentNullException(nameof(visitWindowCalculator));
            _piiService = piiService ?? throw new ArgumentNullException(nameof(piiService));
            _sharedCookieConfiguration = sharedCookieConfiguration ?? throw new ArgumentNullException(nameof(sharedCookieConfiguration)); 
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        public async Task ProcessAsync(CollectedPii collectedPii, CancellationToken token = default)
        {
            var visitExpiration = await _visitWindowCalculator.GetExpirationAsync(collectedPii.UtcTimestamp, token).ConfigureAwait(false);
            var activeVisit = await _visitService.GetByActivityAsync(collectedPii, token).ConfigureAwait(false);
            
            if(activeVisit != null)
            {
                // there is already an active visit
                if(!activeVisit.UtcIdentifiedTimestamp.HasValue
                    || (activeVisit.PiiType.HasValue && activeVisit.PiiType > collectedPii.Pii!.PiiType))
                {
                    // this visit is either unidentified or we have just collected a better form of identification
                    activeVisit.PiiType = collectedPii.Pii!.PiiType;
                    activeVisit.PiiValue = collectedPii.Pii!.Value;
                    activeVisit.UtcIdentifiedTimestamp = collectedPii.UtcTimestamp;
                    
                    await _visitService.UpsertAsync(activeVisit, token).ConfigureAwait(false);
                }                
                else if(activeVisit.PiiType != null 
                    && activeVisit.PiiType == PiiTypeEnum.UserId 
                    && collectedPii.Pii!.PiiType == PiiTypeEnum.UserId
                    && !activeVisit.PiiValue!.Equals(collectedPii.Pii!.Value))
                {
                    // the active visit is already identified with a user id
                    // this is a different user id
                    // so this is a new visit
                    var newVisitForUserId = new Visit()
                    {
                        Id = Guid.NewGuid(),
                        CookieId = collectedPii.CookieId,
                        UtcStart = collectedPii.UtcTimestamp,
                        UtcExpiration = visitExpiration,
                        PiiType = collectedPii.Pii.PiiType,
                        PiiValue = collectedPii.Pii.Value,
                        UtcIdentifiedTimestamp = collectedPii.UtcTimestamp,
                    };

                    await _visitService.UpsertAsync(newVisitForUserId, token).ConfigureAwait(false);
                } // end if       
            }
            else
            {
                // create a new visit
                activeVisit = new Visit()
                {
                    Id = Guid.NewGuid(),
                    CookieId = collectedPii.CookieId,
                    UtcStart = collectedPii.UtcTimestamp,
                    UtcExpiration = visitExpiration,
                    PiiType = collectedPii.Pii!.PiiType,
                    PiiValue = collectedPii.Pii!.Value,
                    UtcIdentifiedTimestamp = collectedPii.UtcTimestamp,
                };

                await _visitService.UpsertAsync(activeVisit, token).ConfigureAwait(false);
            } // end if

            // is this a shared cookie?
            var uniqueUserIds = await _piiService.GetUniqueUserIdsAsync(collectedPii.CookieId, _sharedCookieConfiguration.Threshold, token).ConfigureAwait(false);
            if(uniqueUserIds.Count() < _sharedCookieConfiguration.Threshold)
            {
                // this is not a shared cookie
                // are there any unidentified visits for this cookie?
                var identifiedVisits = await HandleUnidentifiedVisitsAsync(collectedPii, token).ConfigureAwait(false);
                _logger.LogDebug("Identified {Count} visits.", identifiedVisits);
            }
            else
            {
                _logger.LogDebug("Unidentified visits are left anonymous for shared cookies.");
            } // end if
        } // end method
    
        protected async Task<int> HandleUnidentifiedVisitsAsync(CollectedPii collectedPii, CancellationToken token = default)
        {
            var result = 0;
            TPage? page = default;
            TVisitSearchResponse visits;
            do
            {
                // fill the visit searchResponse
                visits = await _visitService.GetUnidentifiedAsync(collectedPii.CookieId, collectedPii.UtcTimestamp, page, token).ConfigureAwait(false);

                if(visits != null && visits.Data != null && visits.Data.Any())
                {
                    foreach(var visit in visits.Data.Where(x => string.IsNullOrWhiteSpace(x.PiiValue)))
                    {
                        visit.PiiValue = collectedPii.Pii!.Value;
                        visit.PiiType = collectedPii.Pii!.PiiType;
                        visit.UtcIdentifiedTimestamp = _clockService.Now;

                        // TODO: move to a batch upsert?
                        await _visitService.UpsertAsync(visit, token).ConfigureAwait(false);
                        result++;
                    } // end foreach
                } // end if

                // increment visitsearch page before next loop.
                if(visits != null && visits.NextPage != null)
                {
                    page = visits.NextPage;
                }
                else
                {
                    page = default;
                } // end if
            } while(visits?.HasMore ?? false);

            return result;
        } // end method
    } // end class
} // end namespace