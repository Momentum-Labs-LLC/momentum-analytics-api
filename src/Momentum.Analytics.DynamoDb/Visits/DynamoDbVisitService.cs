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
            var identifiedVisits = new List<Visit>();

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
                        identifiedVisits.AddRange(visitsWithinTimeRange);
                    } // end if                    
                } // end if

                current = current.Plus(Duration.FromHours(1));
            } while(current < timeRange.UtcEnd);

            if(identifiedVisits.Any())
            {
                result.Data = identifiedVisits;
                result.NextPage = null;
            } // end if

            return result;
        } // end method

        protected virtual async Task<IEnumerable<Visit>> GetHourIdentifiedAsync(
            Instant hour, 
            CancellationToken token = default)
        {
            var result = new List<Visit>();

            Dictionary<string, AttributeValue> nextPage = null;
            do
            {
                var searchResponse = await _visitStorage.GetIdentifiedAsync(hour, nextPage, token).ConfigureAwait(false);
                if(searchResponse != null)
                {
                    nextPage = searchResponse.NextPage;
                    if(searchResponse.Data != null && searchResponse.Data.Any())
                    {
                        result.AddRange(searchResponse.Data);
                    } // end if
                }
                else
                {
                    nextPage = null;
                } // end if
            } while(nextPage != null);

            return result;
        } // end method

        protected virtual async Task<IEnumerable<Visit>> GetHourUnidentifiedAsync(
            Instant hour, 
            CancellationToken token = default)
        {
            var result = new List<Visit>();

            Dictionary<string, AttributeValue> nextPage = null;
            do
            {
                var searchResponse = await _visitStorage.GetUnidentifiedAsync(hour, nextPage, token).ConfigureAwait(false);
                if(searchResponse != null)
                {
                    nextPage = searchResponse.NextPage;
                    if(searchResponse.Data != null && searchResponse.Data.Any())
                    {
                        result.AddRange(searchResponse.Data);
                    } // end if
                }
                else
                {
                    nextPage = null;
                } // end if
            } while(nextPage != null);

            return result;
        } // end method
    } // end class
} // end namespace