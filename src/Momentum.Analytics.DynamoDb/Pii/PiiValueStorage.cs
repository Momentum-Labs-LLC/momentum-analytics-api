using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.DynamoDb.Abstractions;
using Momentum.Analytics.DynamoDb.Client.Interfaces;
using Momentum.Analytics.DynamoDb.Pii.Interfaces;

namespace Momentum.Analytics.DynamoDb.Pii
{
    public class PiiValueStorage : ResourceStorageBase<IPiiValueTableConfiguration>, IPiiValueStorage
    {
        public PiiValueStorage(
                IDynamoDBClientFactory clientFactory, 
                IPiiValueTableConfiguration tableConfiguration, 
                ILogger<PiiValueStorage> logger) 
            : base(clientFactory, tableConfiguration, logger)
        {
        } // end method

        public virtual async Task<PiiValue?> GetByIdAsync(Guid id, CancellationToken token = default)
        {
            PiiValue? result = null;
            var request = new QueryRequest()
            {
                TableName = _tableConfiguration.TableName,
                IndexName = _tableConfiguration.IdIndexName,
                KeyConditionExpression = $"{PiiValueConstants.PII_ID} = :id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>()
                    .AddField(":id", id)
            };

            var client = await _clientFactory.GetAsync(token).ConfigureAwait(false);
            var queryResult = await client.QueryAsync(request, token).ConfigureAwait(false);

            if(queryResult != null && queryResult.Items != null && queryResult.Items.Any())
            {
                result = queryResult.Items.First().ReadPiiValue();
            } // end if

            return result;
        } // end method

        public virtual async Task<PiiValue?> GetByValueAsync(string value, CancellationToken token = default)
        {
            PiiValue? result = null;

            var key = new Dictionary<string, AttributeValue>().AddField(PiiValueConstants.VALUE, value);
            var request = new GetItemRequest(_tableConfiguration.TableName, key);
            var client = await _clientFactory.GetAsync(token).ConfigureAwait(false);
            var getItemResponse = await client.GetItemAsync(request, token).ConfigureAwait(false);

            if(getItemResponse != null && getItemResponse.Item != null && getItemResponse.Item.Any())
            {
                result = getItemResponse.Item.ReadPiiValue();
            } // end if

            return result;
        } // end method

        public virtual async Task InsertAsync(PiiValue piiValue, CancellationToken token = default)
        {
            var request = new PutItemRequest
            {
                TableName = _tableConfiguration.TableName,
                Item = piiValue.ToDynamoDb(),
            };

            var client = await _clientFactory.GetAsync(token).ConfigureAwait(false);
            await client.PutItemAsync(request, token).ConfigureAwait(false);
        } // end method
    } // end class
} // end namespace