using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.PageViews.Models;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.Processing.Cookies;
using Momentum.Analytics.Processing.PageViews.Interfaces;

namespace Momentum.Analytics.Processing.PageViews
{
    public class PageViewProcessor<TPage, TVisitSearchResponse, TVisitService> : IPageViewProcessor
        where TVisitSearchResponse : ISearchResponse<Visit, TPage>
        where TVisitService : IVisitService<TPage, TVisitSearchResponse>
    {
        protected readonly TVisitService _visitService;
        protected readonly ISharedCookieConfiguration _sharedCookieConfiguration;
        protected readonly IClockService _clockService;
        protected readonly ILogger _logger;

        public PageViewProcessor(
            TVisitService visitService,
            ISharedCookieConfiguration sharedCookieConfiguration,
            IClockService clockService,
            ILogger<PageViewProcessor<TPage, TVisitSearchResponse, TVisitService>> logger)
        {
            _visitService = visitService ?? throw new ArgumentNullException(nameof(visitService));
            _sharedCookieConfiguration = sharedCookieConfiguration ?? throw new ArgumentNullException(nameof(sharedCookieConfiguration));
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

            if(hasUpsert)
            {
                await _visitService.UpsertAsync(activeVisit, token).ConfigureAwait(false);
            } // end if
        } // end method
    } // end class
} // end namespace