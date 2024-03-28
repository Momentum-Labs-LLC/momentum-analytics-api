using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.DynamoDb.Abstractions;
using Momentum.Analytics.DynamoDb.Visits.Interfaces;
using Momentum.Analytics.Processing.DynamoDb.Visits.Interfaces;
using Momentum.Analytics.Processing.Visits;
using Momentum.Analytics.Processing.Visits.Interfaces;

namespace Momentum.Analytics.Processing.DynamoDb.Visits
{
    public class DynamoDbIdentifiedVisitProcessor :
        IdentifiedVisitProcessor<Dictionary<string, AttributeValue>, IDynamoSearchResponse<Visit>, IDynamoDbVisitService>,
        IDynamoDbIdentifiedVisitProcessor
    {
        public DynamoDbIdentifiedVisitProcessor(
                IDynamoDbVisitService visitService, 
                IIdentifiedVisitWriter writer,
                ILogger<DynamoDbIdentifiedVisitProcessor> logger)
            : base(visitService, writer, logger)
        {
        } // end method
    } // end class
} // end namespace