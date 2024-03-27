using Microsoft.Extensions.DependencyInjection;
using Momentum.Analytics.Core.Visits.Interfaces;

namespace Momentum.Analytics.Core.Visits
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVisitConfiguration(this IServiceCollection services)
        {
            return services.AddSingleton<IVisitConfiguration, VisitConfiguration>();
        } // end method
    } // end class
} // end namespace