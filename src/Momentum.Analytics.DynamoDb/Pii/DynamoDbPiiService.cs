using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.PII;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.DynamoDb.Abstractions;
using Momentum.Analytics.DynamoDb.Pii.Interfaces;

namespace Momentum.Analytics.DynamoDb.Pii
{
    public class DynamoDbPiiService : PiiService<Dictionary<string, AttributeValue>, IDynamoSearchResponse<CollectedPii>, IDynamoDbCollectedPiiStorage>
    {
        public DynamoDbPiiService(
            IDynamoDbCollectedPiiStorage collectedPiiStorage, 
            IPiiValueStorage piiValueStorage, 
            IEmailHasher emailHasher, 
            ILogger<DynamoDbPiiService> logger) 
            : base(collectedPiiStorage, piiValueStorage, emailHasher, logger)
        {
        } // end method
    } // end class
} // end namespace