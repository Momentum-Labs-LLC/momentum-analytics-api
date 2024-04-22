using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.DynamoDb.Abstractions;
using Momentum.Analytics.DynamoDb.Visits.Interfaces;
using Momentum.Analytics.Processing.Cookies;
using Momentum.Analytics.Processing.DynamoDb.Visits.Interfaces;
using Momentum.Analytics.Processing.Visits;

namespace Momentum.Analytics.Processing.DynamoDb.Visits
{
    public class DynamoDbUnidentifiedVisitProcessor :
        UnidentifiedVisitProcessor<Dictionary<string, AttributeValue>, IDynamoSearchResponse<Visit>, IDynamoDbVisitService>,
        IDynamoDbUnidentifiedVisitProcessor
    {
        public DynamoDbUnidentifiedVisitProcessor(
                IDynamoDbVisitService visitService, 
                IPiiService piiService, 
                ISharedCookieConfiguration sharedCookieConfiguration, 
                ILogger<DynamoDbUnidentifiedVisitProcessor> logger) 
            : base(visitService, piiService, sharedCookieConfiguration, logger)
        {
        } // end method
    } // end class
} // end namespace