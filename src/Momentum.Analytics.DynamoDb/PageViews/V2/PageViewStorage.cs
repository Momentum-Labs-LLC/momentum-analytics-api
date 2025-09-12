using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.PageViews.V2;
using Momentum.Analytics.Core.PageViews.V2.Interfaces;
using Momentum.Analytics.DynamoDb.Abstractions;
using Momentum.Analytics.DynamoDb.Client.Interfaces;

namespace Momentum.Analytics.DynamoDb.PageViews.V2
{
    public class PageViewStorage : ResourceStorageBase<IPageViewV2TableConfiguration>, IPageViewV2Storage
    {
        public PageViewStorage(
                IDynamoDBClientFactory clientFactory, 
                IPageViewV2TableConfiguration tableConfiguration, 
                ILogger<PageViewStorage> logger) 
            : base(clientFactory, tableConfiguration, logger)
        {
        } // end method

        public async Task InsertAsync(PageView pageView, CancellationToken token = default)
        {
            var request = new PutItemRequest
            {
                TableName = _tableConfiguration.TableName,
                Item = pageView.ToDynamoDb()
            };

            var client = await _clientFactory.GetAsync(token).ConfigureAwait(false);
            await client.PutItemAsync(request, token).ConfigureAwait(false);
        } // end method
    } // end class
} // end namespace