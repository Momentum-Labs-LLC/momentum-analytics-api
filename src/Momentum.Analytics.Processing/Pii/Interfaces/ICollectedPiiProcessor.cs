using Momentum.Analytics.Core.PII.Models;

namespace Momentum.Analytics.Processing.Pii.Interfaces
{
    public interface ICollectedPiiProcessor
    {
        Task ProcessAsync(CollectedPii collectedPii, CancellationToken token = default);
    } // end interface
} // end namespace