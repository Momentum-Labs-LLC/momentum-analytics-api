using NodaTime;

namespace Momentum.Analytics.Core.Interfaces
{
    public interface IClockService
    {
        Instant Now { get; }
    } // end interface
} // end namespace