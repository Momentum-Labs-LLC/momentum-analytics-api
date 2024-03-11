using Momentum.Analytics.Core.Interfaces;

namespace Momentum.Analytics.Core.Models
{
    public class TimeRange : ITimeRange
    {
        public DateTime? UtcStart { get; set; }

        public DateTime? UtcEnd { get; set; }
    } // end class
} // end namespace