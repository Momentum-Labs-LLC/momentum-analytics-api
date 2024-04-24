using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.DynamoDb.Abstractions;
using Momentum.Analytics.DynamoDb.Client.Interfaces;
using Momentum.Analytics.DynamoDb.Models;
using Momentum.Analytics.DynamoDb.Pii.Interfaces;

namespace Momentum.Analytics.DynamoDb.Pii
{
    public class CollectedPiiStorage : 
        ResourceStorageBase<ICollectedPiiTableConfiguration>, 
        IDynamoDbCollectedPiiStorage
    {
        public CollectedPiiStorage(
                IDynamoDBClientFactory clientFactory, 
                ICollectedPiiTableConfiguration tableConfiguration, 
                ILogger<CollectedPiiStorage> logger) 
            : base(clientFactory, tableConfiguration, logger)
        {
        } // end method

        public virtual async Task<IEnumerable<CollectedPii>?> GetByCookieIdAsync(Guid cookieId, CancellationToken token = default)
        {
            IEnumerable<CollectedPii>? result = null;
            var request = new QueryRequest()
            {
                TableName = _tableConfiguration.TableName,
                IndexName = _tableConfiguration.CookieTimestampIndexName,
                KeyConditionExpression = $"{CollectedPiiConstants.COOKIE_ID} = :cookie_id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                    .AddField(":cookie_id", cookieId)
            };

            var client = await _clientFactory.GetAsync(token).ConfigureAwait(false);
            var queryResult = await client.QueryAsync(request, token).ConfigureAwait(false);

            if(queryResult != null && queryResult.Items != null && queryResult.Items.Any())
            {
                result = queryResult.Items.Select(x => x.ReadCollectedPii());
            } // end if

            return result;
        } // end method

        public virtual async Task InsertAysnc(CollectedPii collectedPii, CancellationToken token = default)
        {
            var request = new PutItemRequest
            {
                TableName = _tableConfiguration.TableName,
                Item = collectedPii.ToDynamoDb(),
            };

            var client = await _clientFactory.GetAsync(token).ConfigureAwait(false);
            await client.PutItemAsync(request, token).ConfigureAwait(false);
        } // end method

        public virtual async Task<IDynamoSearchResponse<CollectedPii>> GetLatestUserIdsAsync(
            Guid cookieId, 
            int size = 10, 
            Dictionary<string, AttributeValue> page = null, 
            CancellationToken token = default)
        {
            var result = new DynamoSearchResponse<CollectedPii>();

            var request = new QueryRequest()
            {
                TableName = _tableConfiguration.TableName,
                IndexName = _tableConfiguration.CookieTimestampIndexName,
                KeyConditionExpression = $"{CollectedPiiConstants.COOKIE_ID} = :cookie_id",
                FilterExpression = $"{CollectedPiiConstants.PII_TYPE_ID} = :pii_type_id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                    .AddField(":cookie_id", cookieId)
                    .AddField(":pii_type_id", (int)PiiTypeEnum.UserId),
                ScanIndexForward = false // reverse order on timestamp
            };

            var client = await _clientFactory.GetAsync(token).ConfigureAwait(false);
            var queryResult = await client.QueryAsync(request, token).ConfigureAwait(false);

            if(queryResult != null)
            {
                result.NextPage = queryResult.LastEvaluatedKey;

                if(queryResult.Items != null && queryResult.Items.Any())
                {
                    result.Data = queryResult.Items.Select(x => x.ReadCollectedPii()).ToList();
                } // end if                
            } // end if

            return result;
        } // end method
    } // end class
} // end namespace