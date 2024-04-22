using Microsoft.Extensions.DependencyInjection;
using Momentum.Analytics.Core.Visits;
using Momentum.Analytics.DynamoDb.Client;
using Momentum.Analytics.DynamoDb.Visits.Interfaces;

namespace Momentum.Analytics.DynamoDb.Visits
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddDynamoDbVisitService(this IServiceCollection services)
        {
            return services
                .AddDynamoDbClientFactory()
                .AddVisitWindowCalculator()
                .AddSingleton<IVisitTableConfiguration, VisitTableConfiguration>()
                .AddTransient<IDynamoDbVisitStorage, VisitStorage>()
                .AddTransient<IDynamoDbVisitService, DynamoDbVisitService>();
        } // end method
    } // end class
} // end namespace