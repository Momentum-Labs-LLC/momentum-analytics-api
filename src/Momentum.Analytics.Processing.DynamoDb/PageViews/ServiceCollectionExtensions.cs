using Microsoft.Extensions.DependencyInjection;
using Momentum.Analytics.DynamoDb.Pii;
using Momentum.Analytics.DynamoDb.Visits;
using Momentum.Analytics.Processing.DynamoDb.PageViews.Interfaces;

namespace Momentum.Analytics.Processing.DynamoDb.PageViews
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDynamoDbPageViewProcessor(this IServiceCollection services)
        {
            return services
                .AddDynamoDbPiiService()
                .AddDynamoDbVisitService()
                .AddTransient<IDynamoDbPageViewProcessor, DynamoDbPageViewProcessor>();
        } // end method
    } // end class
} // end namespace