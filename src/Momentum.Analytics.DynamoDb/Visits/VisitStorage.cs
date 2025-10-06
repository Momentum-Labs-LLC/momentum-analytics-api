using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.DynamoDb.Abstractions;
using Momentum.Analytics.DynamoDb.Client.Interfaces;
using Momentum.Analytics.DynamoDb.Models;
using Momentum.Analytics.DynamoDb.Visits.Interfaces;
using NodaTime;

namespace Momentum.Analytics.DynamoDb.Visits
{
    public class VisitStorage : 
            ResourceStorageBase<IVisitTableConfiguration>, 
            IDynamoDbVisitStorage
    {
        public VisitStorage(
                IDynamoDBClientFactory clientFactory, 
                IVisitTableConfiguration tableConfiguration, 
                ILogger<VisitStorage> logger) 
            : base(clientFactory, tableConfiguration, logger)
        {
        } // end method

        public virtual Task<IDynamoSearchResponse<Visit>> GetIdentifiedAsync(
            ITimeRange timeRange, 
            Dictionary<string, AttributeValue>? page = default, 
            CancellationToken token = default)
        {
            throw new NotImplementedException();
        } // end method

        public virtual async Task<IDynamoSearchResponse<Visit>> GetIdentifiedAsync(
            Instant hour,
            Dictionary<string, AttributeValue>? page = default,
            CancellationToken token = default)
        {
            var result = new DynamoSearchResponse<Visit>();
            var queryRequest = new QueryRequest()
            {
                TableName = _tableConfiguration.TableName,
                IndexName = _tableConfiguration.IdentifiedIndex,
                ExclusiveStartKey = page ?? new Dictionary<string, AttributeValue>(),
                KeyConditionExpression = $"{VisitConstants.UTC_IDENTIFIED_HOUR} = :hour",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                    .AddField(":hour", hour)
            };

            var client = await _clientFactory.GetAsync(token).ConfigureAwait(false);
            var response = await client.QueryAsync(queryRequest, token).ConfigureAwait(false);

            if(response.Items != null && response.Items.Any())
            {
                result.Data = response.Items.Select(x => x.ToVisit());
                result.NextPage = response.LastEvaluatedKey;
            } // end if

            return result;
        } // end method

        public virtual async Task<Visit?> GetLatestAysnc(Guid cookieId, Instant timestamp, CancellationToken token = default)
        {
            Visit? result = null;
            var queryRequest = new QueryRequest()
            {
                TableName = _tableConfiguration.TableName,
                IndexName = _tableConfiguration.CookieIndex,
                KeyConditionExpression = $"{VisitConstants.COOKIE_ID} = :cookie_id and {VisitConstants.UTC_START} <= :activity",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                    .AddField(":cookie_id", cookieId)
                    .AddField(":activity", timestamp),
                ScanIndexForward = false, // order in descending time
                Limit = 1
            };

            var client = await _clientFactory.GetAsync(token).ConfigureAwait(false);
            var response = await client.QueryAsync(queryRequest, token).ConfigureAwait(false);

            if(response != null && response.Items != null && response.Items.Any())
            {
                result = response.Items.First().ToVisit();
            } // end if

            return result;
        } // end if

        public virtual Task<IDynamoSearchResponse<Visit>> GetUnidentifiedAsync(
            ITimeRange timeRange, 
            Dictionary<string, AttributeValue>? page = default, 
            CancellationToken token = default)
        {
            throw new NotImplementedException();
        } // end method

        public virtual async Task<IDynamoSearchResponse<Visit>> GetUnidentifiedAsync(
            Guid cookieId, 
            Instant timestamp,
            Dictionary<string, AttributeValue>? page = default, 
            CancellationToken token = default)
        {
            var result = new DynamoSearchResponse<Visit>();
            var queryRequest = new QueryRequest()
            {
                TableName = _tableConfiguration.TableName,
                IndexName = _tableConfiguration.CookieIndex,
                ExclusiveStartKey = page ?? new Dictionary<string, AttributeValue>(),
                KeyConditionExpression = $"{VisitConstants.COOKIE_ID} = :cookie_id and {VisitConstants.UTC_START} < :now",
                FilterExpression = $"{VisitConstants.IS_IDENTIFIED} = :no",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                    .AddField(":now", timestamp)
                    .AddField(":cookie_id", cookieId)
                    .AddField(":no", false)
            };

            var client = await _clientFactory.GetAsync(token).ConfigureAwait(false);
            var response = await client.QueryAsync(queryRequest, token).ConfigureAwait(false);

            if(response.Items != null && response.Items.Any())
            {
                result.Data = response.Items.Select(x => x.ToVisit());
                result.NextPage = response.LastEvaluatedKey;
            } // end if

            return result;
        } // end method

        public virtual async Task UpsertAsync(Visit visit, CancellationToken token = default)
        {
            var request = new PutItemRequest
            {
                TableName = _tableConfiguration.TableName,
                Item = visit.ToDynamoDb(),
            };

            var client = await _clientFactory.GetAsync(token).ConfigureAwait(false);
            await client.PutItemAsync(request, token).ConfigureAwait(false);
        } // end method

        public virtual async Task<IDynamoSearchResponse<Visit>> GetUnidentifiedAsync(
            Instant hour, 
            Dictionary<string, AttributeValue>? page = default, 
            CancellationToken token = default)
        {
            var result = new DynamoSearchResponse<Visit>();
            var queryRequest = new QueryRequest()
            {
                TableName = _tableConfiguration.TableName,
                IndexName = _tableConfiguration.VisitStartIndex,
                ExclusiveStartKey = page ?? new Dictionary<string, AttributeValue>(),
                KeyConditionExpression = $"{VisitConstants.UTC_START_HOUR} = :hour",
                FilterExpression = $"{VisitConstants.IS_IDENTIFIED} = :no",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                    .AddField(":hour", hour)                    
                    .AddField(":no", false)
            };

            var client = await _clientFactory.GetAsync(token).ConfigureAwait(false);
            var response = await client.QueryAsync(queryRequest, token).ConfigureAwait(false);

            if(response.Items != null && response.Items.Any())
            {
                result.Data = response.Items.Select(x => x.ToVisit());
                result.NextPage = response.LastEvaluatedKey;
            } // end if

            return result;
        } // end method
    } // end class
} // end namespace