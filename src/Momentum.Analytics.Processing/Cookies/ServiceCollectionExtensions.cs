using Microsoft.Extensions.DependencyInjection;

namespace Momentum.Analytics.Processing.Cookies
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSharedCookieConfiguration(this IServiceCollection services)
        {
            return services.AddSingleton<ISharedCookieConfiguration, SharedCookieConfiguration>();
        } // end method
    } // end class
} // end namespace