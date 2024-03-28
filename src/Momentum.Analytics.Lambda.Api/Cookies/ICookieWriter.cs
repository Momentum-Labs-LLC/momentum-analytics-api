using Momentum.Analytics.Core.Visits.Interfaces;

namespace Momentum.Analytics.Lambda.Api.Cookies
{
    public interface ICookieWriter
    {
        Task SetCookieAsync(Cookie cookie, CancellationToken token = default);
    } // end interface
} // end namespace