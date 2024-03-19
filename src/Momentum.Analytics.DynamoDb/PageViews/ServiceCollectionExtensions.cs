using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Momentum.Analytics.Core.PageViews;
using Momentum.Analytics.Core.PageViews.Interfaces;
using Momentum.Analytics.DynamoDb.Client;
using Momentum.Analytics.DynamoDb.PageViews.Interfaces;

namespace Momentum.Analytics.DynamoDb.PageViews
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPageViewStorage(this IServiceCollection services)
        {
            return services
                .AddDynamoDbClientFactory()
                .AddSingleton<IPageViewTableConfiguration, PageViewTableConfiguration>()
                .AddTransient<IPageViewStorage, PageViewStorage>();
        } // end method

        public static IServiceCollection AddPageViewService(this IServiceCollection services)
        {
            return services
                .AddPageViewStorage()
                .AddTransient<IPageViewService, PageViewService>();
        }
    } // end class
} // end namespace