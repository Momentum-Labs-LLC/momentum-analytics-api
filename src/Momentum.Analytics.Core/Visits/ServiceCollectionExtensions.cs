using Microsoft.Extensions.DependencyInjection;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Visits.Interfaces;

namespace Momentum.Analytics.Core.Visits
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVisitWindowCalculator(this IServiceCollection services)
        {
            return services.AddSingleton<IVisitConfiguration, VisitConfiguration>()
                    .AddTransient<IVisitWindowCalculator, VisitWindowCalculator>();
        } // end method
    } // end class
} // end namespace