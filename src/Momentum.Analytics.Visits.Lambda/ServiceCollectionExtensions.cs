using Microsoft.Extensions.DependencyInjection;
using Momentum.Analytics.DynamoDb.Pii;
using Momentum.Analytics.DynamoDb.Visits;
using Momentum.Analytics.Processing.Cookies;
using Momentum.Analytics.Processing.DynamoDb.Visits;
using Momentum.Analytics.Processing.Visits.Interfaces;
using Momentum.Analytics.Visits.Lambda.IdentifiedVisits;
using Momentum.Analytics.Visits.Lambda.IdentifiedVisits.Interfaces;

namespace Momentum.Analytics.Visits.Lambda
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIdentifiedVisitWriter(this IServiceCollection services)
        {
            return services.AddSingleton<IS3ClientFactory, S3ClientFactory>()
                .AddSingleton<IS3OutputConfiguration, S3OutputConfiguration>()
                .AddSingleton<IColumnNameConfiguration, ColumnNameConfiguration>()
                .AddSingleton<IdentifiedVisitMap>((serviceProvider) => new IdentifiedVisitMap(serviceProvider.GetRequiredService<IColumnNameConfiguration>()))
                .AddTransient<IIdentifiedVisitWriter, S3IdentifiedVisitWriter>();
        } // end method

        public static IServiceCollection AddIdentifiedVisitProcessor(this IServiceCollection services)
        {
            return services
                .AddDynamoDbVisitService()
                .AddIdentifiedVisitWriter()
                .AddTransient<IIdentifiedVisitProcessor, DynamoDbIdentifiedVisitProcessor>();
        } // end method

        public static IServiceCollection AddUnidentifiedVisitProcessor(this IServiceCollection services)
        {
            return services
                .AddDynamoDbVisitService()
                .AddDynamoDbPiiService()
                .AddSharedCookieConfiguration()
                .AddTransient<IUnidentifiedVisitProcessor, DynamoDbUnidentifiedVisitProcessor>();
        } // end method
    } // end class
} // end namespace