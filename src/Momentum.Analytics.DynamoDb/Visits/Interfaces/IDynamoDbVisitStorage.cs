using Amazon.DynamoDBv2.Model;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.DynamoDb.Abstractions;

namespace Momentum.Analytics.DynamoDb.Visits.Interfaces
{
    public interface IDynamoDbVisitStorage : IVisitStorage<Dictionary<string, AttributeValue>, IDynamoSearchResponse<Visit>>
    {
        
    } // end interface
} // end namespace