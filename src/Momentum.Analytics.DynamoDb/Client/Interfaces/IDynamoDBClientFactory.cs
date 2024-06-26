using Amazon.DynamoDBv2;

namespace Momentum.Analytics.DynamoDb.Client.Interfaces
{
    public interface IDynamoDBClientFactory
    {
        Task<IAmazonDynamoDB?> GetAsync(CancellationToken token = default);
    } // end interface
} // end namespace