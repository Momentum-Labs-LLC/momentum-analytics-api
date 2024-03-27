using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Extensions;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.DynamoDb.Abstractions;
using Momentum.Analytics.DynamoDb.Client.Interfaces;
using Momentum.Analytics.DynamoDb.Models;
using Momentum.Analytics.DynamoDb.Visits.Interfaces;

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

        public virtual async Task<Visit?> GetByActivityAsync(Guid cookieId, DateTime utcTimestamp, CancellationToken token = default)
        {
            Visit? result = null;

            var utcToday = DateTime.UtcNow.Trim(TimeSpan.FromDays(1).Ticks);
            var utcTomorrow = utcToday.AddDays(1);
            var queryRequest = new QueryRequest()
            {
                TableName = _tableConfiguration.TableName,
                IndexName = _tableConfiguration.VisitExpirationIndex,
                KeyConditionExpression = $"{VisitConstants.COOKIE_ID} = :cookie_id and {VisitConstants.UTC_EXPIRATION} > :today and {VisitConstants.UTC_EXPIRATION} < :tommorrow",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                    .AddField(":cookie_id", cookieId)
                    .AddField(":today", utcToday)
                    .AddField(":tomorrow", utcTomorrow)
            };

            var client = await _clientFactory.GetAsync(token).ConfigureAwait(false);
            var response = await client.QueryAsync(queryRequest, token).ConfigureAwait(false);

            if(response.Items != null && response.Items.Any())
            {
                result = response.Items.First().ToVisit();                
            } // end if

            return result;
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
                KeyConditionExpression = $"{VisitConstants.IS_IDENTIFIED} = 1 and {VisitConstants.UTC_IDENTIFIED_TIMESTAMP} >= :start and {VisitConstants.UTC_IDENTIFIED_TIMESTAMP} < :end",
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

        public virtual async Task<IDynamoSearchResponse<Visit>> GetUnidentifiedAsync(
            Guid cookieId, 
            Dictionary<string, AttributeValue> page, 
            CancellationToken token = default)
        {
            var result = new DynamoSearchResponse<Visit>();
            var queryRequest = new QueryRequest()
            {
                TableName = _tableConfiguration.TableName,
                IndexName = _tableConfiguration.VisitExpirationIndex,
                ExclusiveStartKey = page,
                KeyConditionExpression = $"{VisitConstants.COOKIE_ID} = :cookie_id and is_identified = 0",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                    .AddField(":cookie_id", cookieId)
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
            var putItemResponse = await client.PutItemAsync(request, token).ConfigureAwait(false);
        } // end method
    } // end class
} // end namespace