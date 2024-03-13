using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Models;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.DynamoDb.Abstractions;
using Momentum.Analytics.DynamoDb.Client.Interfaces;
using Momentum.Analytics.DynamoDb.Visits.Interfaces;

namespace Momentum.Analytics.DynamoDb.Visits
{
    public class VisitStorage : ResourceStorageBase<IVisitTableConfiguration>, IVisitStorage
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

        public virtual async Task<ISearchResponse<Visit>> SearchAsync(IVisitSearchRequest request, CancellationToken token = default)
        {
            var result = new SearchResponse<Visit>();

            string? indexName = null;
            if(request.CookieId.HasValue)
            {
                indexName = _tableConfiguration.VisitExpirationIndex;
                var keyFilter = new Dictionary<string, AttributeValue>()
                {

                };
            }
            else if(request.IsIdentified.HasValue)
            {
                indexName = _tableConfiguration.IdentifiedIndex;
            } // end if

            var queryRequest = new QueryRequest()
            {
                TableName = _tableConfiguration.TableName,
                IndexName = indexName,
                //Key
                // TODO: Build Query
            };

            var client = await _clientFactory.GetAsync(token).ConfigureAwait(false);
            var response = await client.QueryAsync(queryRequest, token).ConfigureAwait(false);

            if(response.Items != null && response.Items.Any())
            {
                result.Data = response.Items.Select(x => x.ToVisit());
                result.HasMore = response.LastEvaluatedKey != null && response.LastEvaluatedKey.Any();
                // TODO: have a way to pass down the last evaluated key for paging
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