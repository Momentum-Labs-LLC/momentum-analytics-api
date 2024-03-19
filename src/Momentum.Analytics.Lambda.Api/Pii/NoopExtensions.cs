using Momentum.Analytics.Core.PII;
using Momentum.Analytics.Core.PII.Interfaces;

namespace Momentum.Analytics.Lambda.Api.Pii
{
    public static class NoopExtensions
    {
        public static IServiceCollection AddNoopPiiService(this IServiceCollection services)
        {
            return services
                .AddTransient<IPiiValueStorage, NoopPiiValueStorage>()
                .AddTransient<ICollectedPiiStorage, NoopCollectedPiiStorage>()
                .AddTransient<IEmailHasher, EmailHasher>()
                .AddTransient<IPiiService, PiiService>();
        } // end method
    } // end class
} // end namespace