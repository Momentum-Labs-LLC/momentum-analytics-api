using Amazon.DynamoDBv2.Model;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.DynamoDb.Abstractions;

namespace Momentum.Analytics.DynamoDb.Pii.Interfaces
{
    public interface IDynamoDbCollectedPiiStorage : 
        ICollectedPiiStorage<Dictionary<string, AttributeValue>, IDynamoSearchResponse<CollectedPii>>
    {
        
    } // end interface
} // end namespace