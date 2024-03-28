using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Visits;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.DynamoDb.Abstractions;
using Momentum.Analytics.DynamoDb.Visits.Interfaces;

namespace Momentum.Analytics.DynamoDb.Visits
{
    public class DynamoDbVisitService : 
        VisitService<Dictionary<string, AttributeValue>, IDynamoSearchResponse<Visit>, IDynamoDbVisitStorage>,
        IDynamoDbVisitService
    {
        public DynamoDbVisitService(
                IVisitExpirationProvider visitExpirationProvider,
                IDynamoDbVisitStorage visitStorage, 
                IMemoryCache memoryCache, 
                ILogger<DynamoDbVisitService> logger) 
            : base(visitExpirationProvider, visitStorage, memoryCache, logger)
        {
        } // end method
    } // end class
} // end namespace