using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.Processing.Visits.Interfaces;

namespace Momentum.Analytics.Processing.Visits
{
    public abstract class IdentifiedVisitProcessor<TPage, TSearchResponse, TVisitService> : IIdentifiedVisitProcessor
        where TSearchResponse : class, ISearchResponse<Visit, TPage>
        where TVisitService : IVisitService<TPage, TSearchResponse>
    {
        protected readonly TVisitService _visitService;
        protected readonly IIdentifiedVisitWriter _writer;
        protected readonly ILogger _logger;

        public IdentifiedVisitProcessor(
            TVisitService visitService,
            IIdentifiedVisitWriter writer,
            ILogger<IdentifiedVisitProcessor<TPage, TSearchResponse, TVisitService>> logger)
        {
            _visitService = visitService ?? throw new ArgumentNullException(nameof(visitService));
            _writer = writer ?? throw new ArgumentNullException(nameof(writer));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        public async Task ReportAsync(ITimeRange timeRange, CancellationToken token = default)
        {
            var identifiedVisits = new List<Visit>();
            TSearchResponse searchResponse = default;
            var nextPage = default(TPage);
            do 
            {
                searchResponse = await _visitService.GetIdentifiedAsync(timeRange, nextPage, token).ConfigureAwait(false);

                nextPage = searchResponse.NextPage;

                if(searchResponse != null && searchResponse.Data != null && searchResponse.Data.Any())
                {
                    identifiedVisits.AddRange(searchResponse.Data);
                } // end if
            } while(searchResponse != null && searchResponse.HasMore);

            if(identifiedVisits.Any())
            {
                await _writer.WriteAsync(timeRange, identifiedVisits, token).ConfigureAwait(false);
            } // end if
        } // end method
    } // end class
} // end namespace