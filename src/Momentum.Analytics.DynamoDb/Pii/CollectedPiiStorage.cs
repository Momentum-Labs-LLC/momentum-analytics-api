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

        public Task<IEnumerable<CollectedPii>?> GetByCookieIdAsync(Guid cookieId, CancellationToken token = default)
        {
            throw new NotImplementedException();
        } // end method

        public virtual async Task InsertAysnc(CollectedPii collectedPii, CancellationToken token = default)
        {
            var request = new PutItemRequest
            {
                TableName = _tableConfiguration.TableName,
                Item = collectedPii.ToDynamoDb(),
            };

            var client = await _clientFactory.GetAsync(token).ConfigureAwait(false);
            var putItemResponse = await client.PutItemAsync(request, token).ConfigureAwait(false);
        } // end method
    } // end class
} // end namespace