using Momentum.Analytics.Core.Interfaces;
using NodaTime;

namespace Momentum.Analytics.Core.Models
{
    public class TimeRange : ITimeRange
    {
        public Instant UtcStart { get; set; }

        public Instant UtcEnd { get; set; }
    } // end class
} // end namespace