using Momentum.Analytics.Core.PII.Models;

namespace Momentum.Analytics.Core.PII.Interfaces
{
    public interface IPiiValueStorage
    {
        Task<PiiValue?> GetByIdAsync(Guid id, CancellationToken token = default);
        Task<PiiValue?> GetByValueAsync(string value, CancellationToken token = default);
        Task InsertAsync(PiiValue piiValue, CancellationToken token = default);
    } // end interface
} // end namespace