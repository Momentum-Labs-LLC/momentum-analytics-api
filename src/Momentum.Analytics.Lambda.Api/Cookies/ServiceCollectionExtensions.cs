namespace Momentum.Analytics.Lambda.Api.Cookies
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCookieWriter(this IServiceCollection services)
        {
            return services
                .AddSingleton<ICookieOptionsFactory, CookieOptionsFactory>()
                .AddHttpContextAccessor()
                .AddTransient<ICookieWriter, CookieWriter>();
        } // end method
    } // end class
} // end namespace