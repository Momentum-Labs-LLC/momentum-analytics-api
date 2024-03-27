using NodaTime;

namespace Momentum.Analytics.Core.Interfaces
{
    public interface ITimeRange
    {
        Instant UtcStart { get; }
        Instant UtcEnd { get; }
    } // end interface
} // end namespace