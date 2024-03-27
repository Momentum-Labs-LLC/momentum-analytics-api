using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.Processing.Pii.Interfaces;
using NodaTime;

namespace Momentum.Analytics.Processing.Pii
{
    public class CollectedPiiProcessor<TPage, TVisitSearchResponse, TVisitService> : ICollectedPiiProcessor
        where TVisitSearchResponse : ISearchResponse<Visit, TPage>
        where TVisitService : IVisitService<TPage, TVisitSearchResponse>
    {
        protected readonly TVisitService _visitService;
        protected readonly IClockService _clockService;
        protected readonly ILogger _logger;

        public CollectedPiiProcessor(
            TVisitService visitService,
            IClockService clockService,
            ILogger<CollectedPiiProcessor<TPage, TVisitSearchResponse, TVisitService>> logger)
        {
            _visitService = visitService ?? throw new ArgumentNullException(nameof(visitService));
            _clockService = clockService ?? throw new ArgumentNullException(nameof(clockService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        public async Task ProcessAsync(CollectedPii collectedPii, CancellationToken token = default)
        {
            var activeVisit = await _visitService.GetByActivityAsync(collectedPii, token).ConfigureAwait(false);
            if(activeVisit == null)
            {
                // create a new visit
                activeVisit = new Visit()
                {
                    Id = Guid.NewGuid(),
                    CookieId = collectedPii.CookieId,
                    UtcStart = collectedPii.UtcTimestamp,
                    PiiType = collectedPii.Pii.PiiType,
                    PiiValue = collectedPii.Pii.Value,
                    UtcIdentifiedTimestamp = collectedPii.UtcTimestamp,
                };

                await _visitService.UpsertAsync(activeVisit, token).ConfigureAwait(false);
            }
            else if(activeVisit.PiiType != null && activeVisit.PiiType >= collectedPii.Pii.PiiType)
            {
                activeVisit.PiiType = collectedPii.Pii.PiiType;
                activeVisit.PiiValue = collectedPii.Pii.Value;
                activeVisit.UtcIdentifiedTimestamp = collectedPii.UtcTimestamp;
                
                await _visitService.UpsertAsync(activeVisit, token).ConfigureAwait(false);
            } // end if

            // are there any unidentified visits for this cookie?
            var identifiedVisits = await HandleUnidentifiedVisitsAsync(collectedPii, token);
            _logger.LogDebug("Identified {Count} visits.", identifiedVisits);
        } // end method
    
        protected async Task<int> HandleUnidentifiedVisitsAsync(CollectedPii collectedPii, CancellationToken token = default)
        {
            var result = 0;
            TPage page = default(TPage);
            TVisitSearchResponse visits;
            do
            {
                // fill the visit searchResponse
                visits = await _visitService.GetUnidentifiedAsync(collectedPii.CookieId, page, token).ConfigureAwait(false);

                if(visits != null && visits.Data != null && visits.Data.Any())
                {
                    foreach(var visit in visits.Data.Where(x => string.IsNullOrWhiteSpace(x.PiiValue)))
                    {
                        visit.PiiValue = collectedPii.Pii.Value;
                        visit.PiiType = collectedPii.Pii.PiiType;
                        visit.UtcIdentifiedTimestamp = _clockService.Now;

                        // TODO: move to a batch upsert?
                        await _visitService.UpsertAsync(visit, token).ConfigureAwait(false);
                        result++;
                    } // end foreach                        
                } // end if

                // increment visitsearch page before next loop.
                page = visits.NextPage;
            } while(visits.HasMore);

            return result;
        } // end method
    } // end class
} // end namespace