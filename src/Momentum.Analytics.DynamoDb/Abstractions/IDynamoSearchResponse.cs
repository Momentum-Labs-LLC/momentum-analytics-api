using Amazon.DynamoDBv2.Model;
using Momentum.Analytics.Core.Interfaces;

namespace Momentum.Analytics.DynamoDb.Abstractions
{
    public interface IDynamoSearchResponse<T> : ISearchResponse<T, Dictionary<string, AttributeValue>>
    {
        
    } // end interface
} // end namespace