using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.DynamoDb.Abstractions;
using Momentum.Analytics.DynamoDb.Visits.Interfaces;
using Momentum.Analytics.Processing.DynamoDb.PageViews.Interfaces;
using Momentum.Analytics.Processing.PageViews;

namespace Momentum.Analytics.Processing.DynamoDb.PageViews
{
    public class DynamoDbPageViewProcessor : 
        PageViewProcessor<Dictionary<string, AttributeValue>, IDynamoSearchResponse<Visit>, IDynamoDbVisitService>,
        IDynamoDbPageViewProcessor
    {
        public DynamoDbPageViewProcessor(
            IPiiService piiService, 
            IDynamoDbVisitService visitService, 
            ILogger<DynamoDbPageViewProcessor> logger) 
            : base(piiService, visitService, logger)
        {
        } // end method
    } // end class
} // end namespace