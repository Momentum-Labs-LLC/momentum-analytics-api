using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.DynamoDb.Abstractions;
using Momentum.Analytics.DynamoDb.Visits.Interfaces;
using Momentum.Analytics.Processing.DynamoDb.Pii.Interfaces;
using Momentum.Analytics.Processing.Pii;

namespace Momentum.Analytics.Processing.DynamoDb.Pii
{
    public class DynamoDbCollectedPiiProcessor : 
        CollectedPiiProcessor<Dictionary<string, AttributeValue>, IDynamoSearchResponse<Visit>, IDynamoDbVisitService>,
        IDynamoDbCollectedPiiProcessor
    {
        public DynamoDbCollectedPiiProcessor(
                IDynamoDbVisitService visitService, 
                ILogger<DynamoDbCollectedPiiProcessor> logger) 
            : base(visitService, logger)
        {
        } // end method
    } // end class
} // end namespace