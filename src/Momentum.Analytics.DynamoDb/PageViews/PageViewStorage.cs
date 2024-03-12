using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.PageViews.Interfaces;
using Momentum.Analytics.Core.PageViews.Models;
using Momentum.Analytics.DynamoDb.Abstractions;
using Momentum.Analytics.DynamoDb.Client.Interfaces;
using Momentum.Analytics.DynamoDb.PageViews.Interfaces;

namespace Momentum.Analytics.DynamoDb.PageViews
{
    public class PageViewStorage : ResourceStorageBase<IPageViewTableConfiguration>, IPageViewStorage
    {
        public PageViewStorage(
                IDynamoDBClientFactory dynamoDBClientFactory,
                IPageViewTableConfiguration pageViewTableConfiguration,
                ILogger<PageViewStorage> logger)
            : base(dynamoDBClientFactory, pageViewTableConfiguration, logger)
        {
        } // end method

        public virtual async Task InsertAsync(PageView pageView, CancellationToken token = default)
        {
            var request = new PutItemRequest
            {
                TableName = _tableConfiguration.TableName,
                Item = pageView.ToDynamoDb(),

                // prevent overwrite of existing record by primary key
                ConditionExpression = "attribute_not_exists(#requestId)",
                ExpressionAttributeNames = new Dictionary<string, string>()
                {
                    { "#requestId", PageViewConstants.REQUEST_ID }
                }
            };

            var client = await _clientFactory.GetAsync(token).ConfigureAwait(false);
            var putItemResponse = await client.PutItemAsync(request, token).ConfigureAwait(false);

            // TODO: handle response
        } // end method
    } // end class
} // end namespace