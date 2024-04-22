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

        public virtual async Task DeleteAsync(Guid id, CancellationToken token = default)
        {
            var request = new DeleteItemRequest()
            {
                TableName = _tableConfiguration.TableName,
                Key = new Dictionary<string, AttributeValue>()
                        .AddField(VisitConstants.ID, id)
            };

            var client = await _clientFactory.GetAsync(token).ConfigureAwait(false);
            var response = await client.DeleteItemAsync(request, token).ConfigureAwait(false);
        } // end method

        public virtual async Task<Visit?> GetAsync(Guid id, CancellationToken token = default)
        {
            var request = new GetItemRequest()
            {
                TableName = _tableConfiguration.TableName,
                Key = new Dictionary<string, AttributeValue>()
                        .AddField(VisitConstants.ID, id)
            };

            var client = await _clientFactory.GetAsync(token).ConfigureAwait(false);
            var response = await client.GetItemAsync(request, token).ConfigureAwait(false);

            return response.Item?.ToVisit();
        } // end method

        public virtual async Task<IDynamoSearchResponse<Visit>> GetIdentifiedAsync(
            ITimeRange timeRange, 
            Dictionary<string, AttributeValue> page, 
            CancellationToken token = default)
        {
            var result = new DynamoSearchResponse<Visit>();
            var queryRequest = new QueryRequest()
            {
                TableName = _tableConfiguration.TableName,
                IndexName = _tableConfiguration.IdentifiedIndex,
                ExclusiveStartKey = page,
                KeyConditionExpression = $"{VisitConstants.UTC_IDENTIFIED_TIMESTAMP} BETWEEN :start and :end",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                    .AddField(":start", timeRange.UtcStart)
                    .AddField(":end", timeRange.UtcEnd)
            };

            var client = await _clientFactory.GetAsync(token).ConfigureAwait(false);
            var response = await client.QueryAsync(queryRequest, token).ConfigureAwait(false);

            if(response.Items != null && response.Items.Any())
            {
                result.Data = response.Items.Select(x => x.ToVisit());
                result.HasMore = response.LastEvaluatedKey != null && response.LastEvaluatedKey.Any();
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
                IndexName = _tableConfiguration.VisitStartIndex,
                KeyConditionExpression = $"{VisitConstants.UTC_START} <= :activity and {VisitConstants.COOKIE_ID} = :cookie_id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                    .AddField(":activity", timestamp)
                    .AddField(":cookie_id", cookieId),
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

        public virtual async Task<IDynamoSearchResponse<Visit>> GetUnidentifiedAsync(
            ITimeRange timeRange, 
            Dictionary<string, AttributeValue> page, 
            CancellationToken token = default)
        {
            var result = new DynamoSearchResponse<Visit>();
            var queryRequest = new QueryRequest()
            {
                TableName = _tableConfiguration.TableName,
                IndexName = _tableConfiguration.VisitStartIndex,
                ExclusiveStartKey = page,
                KeyConditionExpression = $"{VisitConstants.UTC_START} BETWEEN :start and :end",
                FilterExpression = $"{VisitConstants.IS_IDENTIFIED} = :no",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                    .AddField(":start", timeRange.UtcStart)
                    .AddField(":end", timeRange.UtcEnd)
                    .AddField(":no", false)
            };

            var client = await _clientFactory.GetAsync(token).ConfigureAwait(false);
            var response = await client.QueryAsync(queryRequest, token).ConfigureAwait(false);

            if(response.Items != null && response.Items.Any())
            {
                result.Data = response.Items.Select(x => x.ToVisit());
                result.HasMore = response.LastEvaluatedKey != null && response.LastEvaluatedKey.Any();
                result.NextPage = response.LastEvaluatedKey;
            } // end if

            return result;
        } // end method

        public virtual async Task<IDynamoSearchResponse<Visit>> GetUnidentifiedAsync(
            Guid cookieId, 
            Instant timestamp,
            Dictionary<string, AttributeValue> page, 
            CancellationToken token = default)
        {
            var result = new DynamoSearchResponse<Visit>();
            var queryRequest = new QueryRequest()
            {
                TableName = _tableConfiguration.TableName,
                IndexName = _tableConfiguration.VisitStartIndex,
                ExclusiveStartKey = page,
                KeyConditionExpression = $"{VisitConstants.UTC_START} < :now and {VisitConstants.COOKIE_ID} = :cookie_id",
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
                result.HasMore = response.LastEvaluatedKey != null && response.LastEvaluatedKey.Any();
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
    } // end class
} // end namespace