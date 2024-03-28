using Microsoft.Extensions.DependencyInjection;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Visits.Interfaces;

namespace Momentum.Analytics.Core.Visits
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVisitExpirationProvider(this IServiceCollection services)
        {
            return services.AddSingleton<IVisitConfiguration, VisitConfiguration>()
                    .AddTransient<IVisitExpirationProvider, VisitExpirationProvider>();
        } // end method
    } // end class
} // end namespace