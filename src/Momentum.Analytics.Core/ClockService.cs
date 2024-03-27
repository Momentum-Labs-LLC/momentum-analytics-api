using Momentum.Analytics.Core.Interfaces;
using NodaTime;

namespace Momentum.Analytics.Core
{
    public class ClockService : IClockService
    {        
        private readonly IClock _clock;

        public Instant Now => _clock.GetCurrentInstant();

        public ClockService()
            : this(SystemClock.Instance)
        {
        } // end method

        public ClockService(IClock clock)
        {
            _clock = clock;
        } // end method
    } // end class
} // end namespace