using Momentum.Analytics.Core.PII.Models;

namespace Momentum.Analytics.Core.PII.Interfaces
{
    public interface ICollectedPiiStorage
    {
        Task<IEnumerable<CollectedPii>?> GetByCookieIdAsync(Guid cookieId, CancellationToken token = default);
        Task InsertAysnc(CollectedPii collectedPii, CancellationToken token = default); 
    } // end interface
} // end namespace