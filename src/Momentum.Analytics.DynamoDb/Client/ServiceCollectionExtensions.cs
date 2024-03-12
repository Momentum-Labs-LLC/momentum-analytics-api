using Microsoft.Extensions.DependencyInjection;
using Momentum.Analytics.DynamoDb.Client.Interfaces;

namespace Momentum.Analytics.DynamoDb.Client
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDynamoDbClientFactory(this IServiceCollection services)
        {
            return services.AddSingleton<IDynamoDBClientFactory, DynamoDbClientFactory>();
        } // end method
    } // end class
} // end namespace