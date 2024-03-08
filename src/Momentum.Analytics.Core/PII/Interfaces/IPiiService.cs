using Momentum.Analytics.Core.PII.Models;

namespace Momentum.Analytics.Core.PII.Interfaces
{
    public interface IPiiService
    {
        Task<PiiValue?> GetPiiAsync(string value, CancellationToken token = default);
        Task<IEnumerable<PiiValue>> GetByCookieIdAsync(Guid cookieId, CancellationToken token = default);
        Task RecordAsync(CollectedPii collectedPii, CancellationToken token = default);
    } // end interface
} // end namespace