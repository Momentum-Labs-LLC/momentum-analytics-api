using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Momentum.Analytics.Core.PII;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.DynamoDb.Client;
using Momentum.Analytics.DynamoDb.Pii.Interfaces;

namespace Momentum.Analytics.DynamoDb.Pii
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDynamoDbPiiService(this IServiceCollection services)
        {
            return services
                .AddDynamoDbClientFactory()
                .AddSingleton<ICollectedPiiTableConfiguration, CollectedPiiTableConfiguration>()
                .AddTransient<ICollectedPiiStorage, CollectedPiiStorage>()
                .AddSingleton<IPiiValueTableConfiguration, PiiValueTableConfiguration>()
                .AddTransient<IPiiValueStorage, PiiValueStorage>()
                .AddTransient<IPiiService, PiiService>();
        } // end method
    } // end class
} // end namespace