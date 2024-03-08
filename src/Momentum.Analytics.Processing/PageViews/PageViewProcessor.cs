using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.PageViews.Interfaces;
using Momentum.Analytics.Core.PageViews.Models;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.Visits;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Processing.PageViews.Interfaces;

namespace Momentum.Analytics.Processing.PageViews
{
    public class PageViewProcessor : IPageViewProcessor
    {
        protected readonly IPiiService _piiService;
        protected readonly IIdentifiedVisitService _visitService;
        protected readonly ILogger _logger;

        public PageViewProcessor(
            IPiiService piiService,
            IIdentifiedVisitService visitService,
            ILogger<PageViewProcessor> logger)
        {
            _piiService = piiService ?? throw new ArgumentNullException(nameof(piiService));
            _visitService = visitService ?? throw new ArgumentNullException(nameof(visitService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        public virtual async Task ProcessAsync(PageView pageView, CancellationToken token = default)
        {
            var piiValues = await _piiService.GetByCookieIdAsync(pageView.CookieId, token).ConfigureAwait(false);
            
            if(piiValues != null && piiValues.Any())
            {
                var preferredPii = piiValues.OrderBy(x => x.PiiType).First();
                var activeVisit = await _visitService.GetActiveVisitAsync(preferredPii, token).ConfigureAwait(false);
                if(activeVisit != null)
                {
                    if(activeVisit.FunnelStep < pageView.FunnelStep)
                    {
                        await _visitService.UpdateFunnelStepAsync(activeVisit, pageView.FunnelStep, token).ConfigureAwait(false);
                    } // end if
                } 
                else
                {
                    await _visitService.CreateVisitAsync(preferredPii, new List<PageView>() { pageView }, token).ConfigureAwait(false);
                } // end if
            } // end if
        } // end method
    } // end class
} // end namespace