using Amazon.DynamoDBv2.Model;
using Momentum.Analytics.Core.Models;
using Momentum.Analytics.DynamoDb.Abstractions;

namespace Momentum.Analytics.DynamoDb.Models
{
    public class DynamoSearchResponse<T> :
        SearchResponse<T, Dictionary<string, AttributeValue>>,
        IDynamoSearchResponse<T>
    {
        public override bool HasMore { get { return NextPage != null && NextPage.Any(); } }
    } // end class
} // end namespace