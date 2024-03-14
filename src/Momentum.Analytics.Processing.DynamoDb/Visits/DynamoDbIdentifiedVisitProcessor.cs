using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.DynamoDb.Abstractions;
using Momentum.Analytics.DynamoDb.Visits.Interfaces;
using Momentum.Analytics.Processing.DynamoDb.Visits.Interfaces;
using Momentum.Analytics.Processing.Visits;

namespace Momentum.Analytics.Processing.DynamoDb.Visits
{
    public abstract class DynamoDbIdentifiedVisitProcessor :
        IdentifiedVisitProcessor<Dictionary<string, AttributeValue>, IDynamoSearchResponse<Visit>, IDynamoDbVisitService>,
        IDynamoDbIdentifiedVisitProcessor
    {
        public DynamoDbIdentifiedVisitProcessor(
                IDynamoDbVisitService visitService, 
                ILogger<DynamoDbIdentifiedVisitProcessor> logger)
            : base(visitService, logger)
        {
        } // end method
    } // end class
} // end namespace