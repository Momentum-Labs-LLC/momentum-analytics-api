using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Extensions;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Visits;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.DynamoDb.Abstractions;
using Momentum.Analytics.DynamoDb.Models;
using Momentum.Analytics.DynamoDb.Visits.Interfaces;
using NodaTime;

namespace Momentum.Analytics.DynamoDb.Visits
{
    public class DynamoDbVisitService : 
        VisitService<Dictionary<string, AttributeValue>, IDynamoSearchResponse<Visit>, IDynamoDbVisitStorage>,
        IDynamoDbVisitService
    {
        public DynamoDbVisitService(
                IVisitWindowCalculator visitWindowCalculator,
                IDynamoDbVisitStorage visitStorage, 
                ILogger<DynamoDbVisitService> logger) 
            : base(visitWindowCalculator, visitStorage, logger)
        {
        } // end method

        public override async Task<IDynamoSearchResponse<Visit>> GetIdentifiedAsync(
            ITimeRange timeRange, 
            Dictionary<string, AttributeValue> page, 
            CancellationToken token = default)
        {
            var result = new DynamoSearchResponse<Visit>();
            var identifiedVisits = new List<Visit>();

            Instant current = timeRange.UtcStart; // TODO: Truncate to hour utc
            do
            {
                var hour = current.TrimToHour();
                var hoursIdentifiedVisits = await GetHourIdentifiedAsync(hour, token).ConfigureAwait(false);

                if(hoursIdentifiedVisits != null && hoursIdentifiedVisits.Any())
                {
                    var visitsWithinTimeRange = hoursIdentifiedVisits.Where(x => 
                            x.UtcIdentifiedTimestamp >= timeRange.UtcStart
                            && x.UtcIdentifiedTimestamp <= timeRange.UtcEnd);
                    if(visitsWithinTimeRange.Any())
                    {
                        _logger.LogDebug("Adding {0} identified visits.", visitsWithinTimeRange.Count());
                        identifiedVisits.AddRange(visitsWithinTimeRange);
                    } // end if
                } // end if

                current = current.Plus(Duration.FromHours(1));
            } while(current < timeRange.UtcEnd);

            if(identifiedVisits.Any())
            {
                result.Data = identifiedVisits;
            } // end if

            return result;
        } // end method

        public override async Task<IDynamoSearchResponse<Visit>> GetUnidentifiedAsync(
            ITimeRange timeRange, 
            Dictionary<string, AttributeValue> page, 
            CancellationToken token = default)
        {
            var result = new DynamoSearchResponse<Visit>();
            var unidentifiedVisits = new List<Visit>();

            Instant current = timeRange.UtcStart; // TODO: Truncate to hour utc
            do
            {
                var hour = current.TrimToHour();                
                var hoursIdentifiedVisits = await GetHourUnidentifiedAsync(hour, token).ConfigureAwait(false);

                if(hoursIdentifiedVisits != null && hoursIdentifiedVisits.Any())
                {
                    var visitsWithinTimeRange = hoursIdentifiedVisits.Where(x => 
                            x.UtcIdentifiedTimestamp >= timeRange.UtcStart
                            && x.UtcIdentifiedTimestamp <= timeRange.UtcEnd);
                    if(visitsWithinTimeRange.Any())
                    {
                        _logger.LogDebug("Adding {0} unidentified visits.", visitsWithinTimeRange.Count());
                        unidentifiedVisits.AddRange(visitsWithinTimeRange);
                    } // end if                    
                } // end if

                current = current.Plus(Duration.FromHours(1));
            } while(current < timeRange.UtcEnd);

            if(unidentifiedVisits.Any())
            {
                result.Data = unidentifiedVisits;
                result.NextPage = null;
            } // end if

            return result;
        } // end method

        protected virtual async Task<IEnumerable<Visit>> GetHourIdentifiedAsync(
            Instant hour, 
            CancellationToken token = default)
        {
            var result = new List<Visit>();

            int pageCounter = 0;
            Dictionary<string, AttributeValue> nextPage = null;
            IDynamoSearchResponse<Visit> searchResponse;
            do
            {
                pageCounter++;
                _logger.LogDebug("Retrieving identified visits for hour {0} - page {1}", hour.InUtc().ToString("yyyy-MM-dd HH:mm:ss", null), pageCounter);
                searchResponse = await _visitStorage.GetIdentifiedAsync(hour, nextPage, token).ConfigureAwait(false);

                if(searchResponse.Data != null && searchResponse.Data.Any())
                {
                    _logger.LogDebug("{0} identified visits found in hour {1}", searchResponse.Data.Count(), hour.InUtc().ToString("yyyy-MM-dd HH:mm:ss", null));
                    result.AddRange(searchResponse.Data);
                } // end if

                nextPage = searchResponse.NextPage;
            } while(searchResponse.HasMore);

            return result;
        } // end method

        protected virtual async Task<IEnumerable<Visit>> GetHourUnidentifiedAsync(
            Instant hour, 
            CancellationToken token = default)
        {
            var result = new List<Visit>();

            var pageCounter = 0;
            Dictionary<string, AttributeValue> nextPage = null;
            IDynamoSearchResponse<Visit> searchResponse;
            do
            {
                pageCounter++;
                _logger.LogDebug("Retrieving unidentified visits for hour {0} - page {1}", hour.InUtc().ToString("yyyy-MM-dd HH:mm:ss", null), pageCounter);
                searchResponse = await _visitStorage.GetUnidentifiedAsync(hour, nextPage, token).ConfigureAwait(false);
                if(searchResponse.Data != null && searchResponse.Data.Any())
                {
                    _logger.LogDebug("{0} unidentified visits found in hour {1}", searchResponse.Data.Count(), hour.InUtc().ToString("yyyy-MM-dd HH:mm:ss", null));
                    result.AddRange(searchResponse.Data);
                } // end if

                nextPage = searchResponse.NextPage;
            } while(searchResponse.HasMore);

            return result;
        } // end method
    } // end class
} // end namespace