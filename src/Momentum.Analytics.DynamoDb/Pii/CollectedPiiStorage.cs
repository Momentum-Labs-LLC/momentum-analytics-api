using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.DynamoDb.Abstractions;
using Momentum.Analytics.DynamoDb.Client.Interfaces;
using Momentum.Analytics.DynamoDb.Pii.Interfaces;

namespace Momentum.Analytics.DynamoDb.Pii
{
    public class CollectedPiiStorage : ResourceStorageBase<ICollectedPiiTableConfiguration>, ICollectedPiiStorage
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
    } // end class
} // end namespace