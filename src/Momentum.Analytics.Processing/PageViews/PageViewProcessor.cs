using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.PageViews.Models;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.Processing.PageViews.Interfaces;
using NodaTime;

namespace Momentum.Analytics.Processing.PageViews
{
    public class PageViewProcessor<TPage, TVisitSearchResponse, TVisitService> : IPageViewProcessor
        where TVisitSearchResponse : ISearchResponse<Visit, TPage>
        where TVisitService : IVisitService<TPage, TVisitSearchResponse>
    {
        protected readonly IPiiService _piiService;
        protected readonly TVisitService _visitService;
        protected readonly IClockService _clockService;
        protected readonly ILogger _logger;

        public PageViewProcessor(
            IPiiService piiService,
            TVisitService visitService,
            IClockService clockService,
            ILogger<PageViewProcessor<TPage, TVisitSearchResponse, TVisitService>> logger)
        {
            _piiService = piiService ?? throw new ArgumentNullException(nameof(piiService));
            _visitService = visitService ?? throw new ArgumentNullException(nameof(visitService));
            _clockService = clockService ?? throw new ArgumentNullException(nameof(clockService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        public virtual async Task ProcessAsync(PageView pageView, CancellationToken token = default)
        {   
            var hasUpsert = false;
            var activeVisit = await _visitService.GetByActivityAsync(pageView, token).ConfigureAwait(false);

            var visitExpiration = await _visitService.GetVisitExpirationAsync(pageView.UtcTimestamp, token).ConfigureAwait(false);
            if(activeVisit == null)
            {
                hasUpsert = true;
                activeVisit = new Core.Visits.Models.Visit()
                {
                    Id = Guid.NewGuid(),
                    CookieId = pageView.CookieId,
                    UtcStart = pageView.UtcTimestamp,
                    UtcExpiration = visitExpiration,
                    FunnelStep = pageView.FunnelStep                    
                };
            }

            if(activeVisit.UtcExpiration < visitExpiration)
            {
                activeVisit.UtcExpiration = visitExpiration;
                hasUpsert = true;
            } // end if

            if(activeVisit.UtcStart > pageView.UtcTimestamp)
            {
                activeVisit.UtcStart = pageView.UtcTimestamp;
                hasUpsert = true;
            } // end if

            if(activeVisit.FunnelStep < pageView.FunnelStep)
            {
                activeVisit.FunnelStep = pageView.FunnelStep;
                hasUpsert = true;
            } // end if

            if(string.IsNullOrWhiteSpace(activeVisit.Referer)
                && !string.IsNullOrWhiteSpace(pageView.Referer))
            {
                activeVisit.Referer = pageView.Referer;
                activeVisit.Source = pageView.UtmParameters?.FirstOrDefault(x => x.Parameter == UrchinParameterEnum.Source)?.Value;
                activeVisit.Medium = pageView.UtmParameters?.FirstOrDefault(x => x.Parameter == UrchinParameterEnum.Medium)?.Value;

                hasUpsert = true;
            } // end if
            
            if(activeVisit.PiiType == null || activeVisit.PiiType > PiiTypeEnum.UserId)
            {
                var piiValues = await _piiService.GetByCookieIdAsync(pageView.CookieId, token).ConfigureAwait(false);
                if(piiValues != null && piiValues.Any())
                {
                    var preferredPii = piiValues.OrderBy(x => x.PiiType).First();
                    
                    if(preferredPii.PiiType < activeVisit.PiiType)
                    {
                        activeVisit.PiiValue = preferredPii.Value;
                        activeVisit.PiiType = preferredPii.PiiType;
                        activeVisit.UtcIdentifiedTimestamp = _clockService.Now;

                        hasUpsert = true;
                    } // end if
                } // end if
            } // end if

            if(hasUpsert)
            {
                await _visitService.UpsertAsync(activeVisit, token).ConfigureAwait(false);
            } // end if
        } // end method
    } // end class
} // end namespace