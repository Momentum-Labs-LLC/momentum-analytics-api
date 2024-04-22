using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.DynamoDb.Abstractions;
using Momentum.Analytics.DynamoDb.Visits.Interfaces;
using Momentum.Analytics.Processing.Cookies;
using Momentum.Analytics.Processing.DynamoDb.PageViews.Interfaces;
using Momentum.Analytics.Processing.PageViews;

namespace Momentum.Analytics.Processing.DynamoDb.PageViews
{
    public class DynamoDbPageViewProcessor : 
        PageViewProcessor<Dictionary<string, AttributeValue>, IDynamoSearchResponse<Visit>, IDynamoDbVisitService>,
        IDynamoDbPageViewProcessor
    {
        public DynamoDbPageViewProcessor(
            IDynamoDbVisitService visitService,
            ISharedCookieConfiguration sharedCookieConfiguration,
            IClockService clockService, 
            ILogger<DynamoDbPageViewProcessor> logger) 
            : base(visitService, sharedCookieConfiguration, clockService, logger)
        {
        } // end method
    } // end class
} // end namespace