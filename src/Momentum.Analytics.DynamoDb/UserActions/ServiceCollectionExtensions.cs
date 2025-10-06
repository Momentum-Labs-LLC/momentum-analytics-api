using Microsoft.Extensions.DependencyInjection;
using Momentum.Analytics.Core.UserActions;
using Momentum.Analytics.Core.UserActions.Interfaces;
using Momentum.Analytics.DynamoDb.Client;

namespace Momentum.Analytics.DynamoDb.UserActions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddFieldChangeService(this IServiceCollection services)
    {
        return services
            .AddDynamoDbClientFactory()
            .AddSingleton<IFieldChangeTableConfiguration, FieldChangeTableConfiguration>()
            .AddTransient<IFieldChangeStorage, FieldChangeStorage>()
            .AddTransient<IFieldChangeService, FieldChangeService>();
    } // end method
} // end class