using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.PageViews.Interfaces;
using Momentum.Analytics.Core.PageViews.Models;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.Core.Visits;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.Processing.Pii.Interfaces;

namespace Momentum.Analytics.Processing.Pii
{
    public class CollectedPiiProcessor : ICollectedPiiProcessor
    {
        protected readonly IVisitService _visitService;        
        protected readonly ILogger _logger;

        public CollectedPiiProcessor(
            IVisitService visitService,            
            ILogger<CollectedPiiProcessor> logger)
        {
            _visitService = visitService ?? throw new ArgumentNullException(nameof(visitService));            
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
            var visitSearch = new VisitSearchRequest() 
            { 
                CookieId = collectedPii.CookieId,
                IsIdentified = false
            };
            ISearchResponse<Visit> visits = null;
            do
            {
                // fill the visit searchResponse
                visits = await _visitService.SearchAsync(visitSearch, token).ConfigureAwait(false);

                if(visits != null && visits.Data != null && visits.Data.Any())
                {
                    foreach(var visit in visits.Data.Where(x => string.IsNullOrWhiteSpace(x.PiiValue)))
                    {
                        visit.PiiValue = collectedPii.Pii.Value;
                        visit.PiiType = collectedPii.Pii.PiiType;
                        visit.UtcIdentifiedTimestamp = DateTime.UtcNow;

                        // TODO: move to a batch upsert?
                        await _visitService.UpsertAsync(visit, token).ConfigureAwait(false);
                        result++;
                    } // end foreach                        
                } // end if

                // increment visitsearch page before next loop.
                visitSearch.Page += 1;
            } while(visits.HasMore);

            return result;
        } // end method
    } // end class
} // end namespace