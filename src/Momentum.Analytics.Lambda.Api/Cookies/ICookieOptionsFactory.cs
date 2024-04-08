namespace Momentum.Analytics.Lambda.Api.Cookies
{
    public interface ICookieOptionsFactory
    {
        Task<CookieOptions> GetAsync(CancellationToken token = default);
    } // end interface
} // end namespace