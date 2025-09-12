using Microsoft.Extensions.DependencyInjection;
using Momentum.Analytics.Core.PageViews.V2;
using Momentum.Analytics.Core.PageViews.V2.Interfaces;
using Momentum.Analytics.DynamoDb.Client;

namespace Momentum.Analytics.DynamoDb.PageViews.V2
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPageViewV2Storage(this IServiceCollection services)
        {
            return services
                .AddDynamoDbClientFactory()
                .AddSingleton<IPageViewV2TableConfiguration, PageViewTableConfiguration>()
                .AddTransient<IPageViewV2Storage, PageViewStorage>();
        } // end method

        public static IServiceCollection AddPageViewV2Service(this IServiceCollection services)
        {
            return services
                .AddPageViewV2Storage()
                .AddTransient<IPageViewV2Service, PageViewService>();
        } // end method
    } // end class
} // end namespace