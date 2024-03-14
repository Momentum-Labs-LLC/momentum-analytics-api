using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.Processing.Visits.Interfaces;

namespace Momentum.Analytics.Processing.Visits
{
    public abstract class IdentifiedVisitProcessor<TPage, TSearchResponse, TVisitService> : IIdentifiedVisitProcessor
        where TSearchResponse : ISearchResponse<Visit, TPage>
        where TVisitService : IVisitService<TPage, TSearchResponse>
    {
        protected readonly TVisitService _visitService;
        protected readonly ILogger _logger;

        public IdentifiedVisitProcessor(
            TVisitService visitService,
            ILogger<IdentifiedVisitProcessor<TPage, TSearchResponse, TVisitService>> logger)
        {
            _visitService = visitService ?? throw new ArgumentNullException(nameof(visitService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        public async Task ExportAsync(ITimeRange timeRange, CancellationToken token = default)
        {
            var identifiedVisits = new List<Visit>();
            TSearchResponse searchResponse = default(TSearchResponse);
            do 
            { 
                searchResponse = await _visitService.GetIdentifiedAsync(timeRange, searchResponse.NextPage, token).ConfigureAwait(false);

                if(searchResponse != null && searchResponse.Data != null && searchResponse.Data.Any())
                {
                    identifiedVisits.AddRange(searchResponse.Data);
                } // end if
            } while(searchResponse == null || searchResponse.HasMore);

            if(identifiedVisits.Any())
            {
                await WriteAsync(identifiedVisits, token).ConfigureAwait(false);
            } // end if
        } // end method
        
        protected abstract Task WriteAsync(IEnumerable<Visit> identifiedVisits, CancellationToken token = default);
    } // end class
} // end namespace