using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.Processing.Cookies;
using Momentum.Analytics.Processing.Visits.Interfaces;

namespace Momentum.Analytics.Processing.Visits
{
    public class UnidentifiedVisitProcessor<TPage, TSearchResponse, TVisitService> : IUnidentifiedVisitProcessor
        where TSearchResponse : class, ISearchResponse<Visit, TPage>
        where TVisitService : IVisitService<TPage, TSearchResponse>
    {
        protected readonly TVisitService _visitService;
        protected readonly IPiiService _piiService;
        protected readonly ISharedCookieConfiguration _sharedCookieConfiguration;
        protected readonly ILogger _logger;

        public UnidentifiedVisitProcessor(
            TVisitService visitService,
            IPiiService piiService,
            ISharedCookieConfiguration sharedCookieConfiguration,
            ILogger<UnidentifiedVisitProcessor<TPage, TSearchResponse, TVisitService>> logger) 
        {
            _visitService = visitService ?? throw new ArgumentNullException(nameof(visitService));
            _piiService = piiService ?? throw new ArgumentNullException(nameof(piiService));
            _sharedCookieConfiguration = sharedCookieConfiguration ?? throw new ArgumentNullException(nameof(sharedCookieConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        public virtual async Task ProcessAsync(ITimeRange timeRange, CancellationToken token = default)
        {
            TSearchResponse searchResponse = default;
            var nextPage = default(TPage);
            do
            {
                searchResponse = await _visitService.GetUnidentifiedAsync(timeRange, nextPage, token).ConfigureAwait(false);

                nextPage = searchResponse.NextPage;

                if(searchResponse != null && searchResponse.Data != null && searchResponse.Data.Any())
                {
                    await Parallel.ForEachAsync(searchResponse.Data, 
                        async (visit, token) => await ProcessVisitAsync(visit, token).ConfigureAwait(false));
                } // end if
            } while(searchResponse != default && searchResponse.HasMore);
        } // end method

        protected virtual async Task ProcessVisitAsync(Visit visit, CancellationToken token = default)
        {
            var uniqueUserIds = await _piiService.GetUniqueUserIdsAsync(visit.CookieId, _sharedCookieConfiguration.Threshold, token).ConfigureAwait(false);

            if(uniqueUserIds != null 
                && uniqueUserIds.Any()
                && uniqueUserIds.Count() < _sharedCookieConfiguration.Threshold) 
            {
                var identifiedVisits = uniqueUserIds.Select(x => new Visit()
                {
                    Id = Guid.NewGuid(),
                    CookieId = visit.CookieId,
                    UtcStart = visit.UtcStart,
                    UtcExpiration = visit.UtcExpiration,
                    FunnelStep = visit.FunnelStep,
                    Referer = visit.Referer,
                    Source = visit.Source,
                    Medium = visit.Medium,
                    PiiValue = x.Pii.Value,
                    PiiType = PiiTypeEnum.UserId,
                    UtcIdentifiedTimestamp = visit.UtcExpiration
                });

                identifiedVisits.First().Id = visit.Id;

                await Parallel.ForEachAsync(identifiedVisits, async(identifiedVisit, token) =>
                        await _visitService.UpsertAsync(identifiedVisit, token).ConfigureAwait(false)) 
                    .ConfigureAwait(false);
            } // end if
        } // end method
    } // end class
} // end namespace