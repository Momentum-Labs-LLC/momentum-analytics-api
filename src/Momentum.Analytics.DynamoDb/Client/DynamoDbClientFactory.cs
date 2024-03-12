using Amazon.DynamoDBv2;
using Momentum.Analytics.DynamoDb.Client.Interfaces;

namespace Momentum.Analytics.DynamoDb.Client
{
    public class DynamoDbClientFactory : IDynamoDBClientFactory
    {
        public virtual async Task<IAmazonDynamoDB?> GetAsync(CancellationToken token = default)
        {
            return new AmazonDynamoDBClient();
        } // end method
    } // end class
} // end namespace