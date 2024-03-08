using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.PageViews.Interfaces;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.Core.Visits;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Processing.Pii.Interfaces;

namespace Momentum.Analytics.Processing.Pii
{
    public class CollectedPiiProcessor : ICollectedPiiProcessor
    {
        protected readonly IPageViewService _pageViewService;
        protected readonly IPiiService _piiService;
        protected readonly IIdentifiedVisitService _visitService;        
        protected readonly ILogger _logger;

        public CollectedPiiProcessor(
            IPageViewService pageViewService,
            IPiiService piiService,
            IIdentifiedVisitService visitService,            
            ILogger<CollectedPiiProcessor> logger)
        {
            _pageViewService = pageViewService ?? throw new ArgumentNullException(nameof(pageViewService));
            _piiService = piiService ?? throw new ArgumentNullException(nameof(piiService));
            _visitService = visitService ?? throw new ArgumentNullException(nameof(visitService));            
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        public async Task ProcessAsync(CollectedPii collectedPii, CancellationToken token = default)
        {
            var doesPiiAlreadyExistForThisCookie = false;
            var existingPii = await _piiService.GetByCookieIdAsync(collectedPii.CookieId, token).ConfigureAwait(false);            

            if(existingPii != null && existingPii.Any())
            {
                // remove the Pii we are doing processing for as it is "new"
                existingPii = existingPii.Where(x => x.Id != collectedPii.Pii.Id);
            } // end if

            if(existingPii == null || !existingPii.Any())
            {
                // this is the first pii received for this cookie
                var pageViews = await _pageViewService.GetByCookieAsync(collectedPii.CookieId, token).ConfigureAwait(false);

                // are there any pageviews?
                if(pageViews != null && pageViews.Any())
                {
                    // group by visit periods
                    var visitGroups = pageViews.GroupBy(x => x.UtcTimestamp.Date);
                    foreach(var visitGroup in visitGroups)
                    {
                        // create a visit for each group
                        await _visitService.CreateVisitAsync(collectedPii.Pii, visitGroup, token).ConfigureAwait(false);
                    } // end method
                } // end if
            }
            else if(!existingPii.Any(x => x.Value.Equals(collectedPii.Pii.Value)))
            {
                // this is new pii for a cookie that already has other pii
                // get the most important pii that we currently have
                var orderedPii = existingPii.OrderBy(x => x.PiiType);

                IdentifiedVisit activeVisit = null;

                // foreach piece of pii that already exists
                foreach(var pii in orderedPii)
                {
                    // get the visit currently being recorded under the pii
                    activeVisit = await _visitService.GetActiveVisitAsync(pii, token).ConfigureAwait(false);

                    // if there is an active visit associated with this 
                    if(activeVisit != null
                        // and the collected pii we are processing is more important than the pii associated with the active visit.
                      && collectedPii.Pii.PiiType <= activeVisit.PiiType)
                    {
                        await _visitService.UpdatePiiAsync(activeVisit, collectedPii.Pii, token).ConfigureAwait(false);
                    } // end if
                } // end foreach
            }
            else
            {
                _logger.LogDebug("This pii already exists with this cookie.");
            } // end if
        } // end method
    } // end class
} // end namespace