using Microsoft.Extensions.Configuration;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Models;
using Momentum.Analytics.Core.Visits.Interfaces;
using NodaTime;

namespace Momentum.Analytics.Visits.Lambda
{
    public class IdentifiedVisitTimeRangeProvider : IIdentifiedVisitTimeRangeProvider
    {
        public const string TRIM_TO_HOUR = "TRIM_TO_HOUR";
        public const bool TRIM_TO_HOUR_DEFAULT = true;
        public const string HOURS_LOOKBACK = "HOURS_LOOKBACK";
        public const int HOURS_LOOKBACK_DEFAULT = 24;
        public ITimeRange TimeRange { get; protected set; }

        public bool TrimToHour { get; protected set; }

        public IdentifiedVisitTimeRangeProvider(IConfiguration configuration, IVisitConfiguration visitConfiguration, IClockService clockService)
        {
            var now = clockService.Now;
            var hoursLookback = configuration.GetValue<int>(HOURS_LOOKBACK, HOURS_LOOKBACK_DEFAULT);

            var trimToHour = configuration.GetValue<bool>(TRIM_TO_HOUR, TRIM_TO_HOUR_DEFAULT);

            var zoned = now.InUtc();
            if(visitConfiguration.TimeZone != null)
            {
                zoned = now.InZone(visitConfiguration.TimeZone);
            } // end if

            var endInstant = zoned.ToInstant();
            if(trimToHour)
            {
                // get the time of day trimed to hour
                var specificHour = zoned.TimeOfDay.With(TimeAdjusters.TruncateToHour);

                var nanosecondsIntoHour = zoned.TimeOfDay.NanosecondOfDay - specificHour.NanosecondOfDay;
                // get the zoned time trimed to the hour
                var zonedHour = zoned.Minus(Duration.FromNanoseconds(nanosecondsIntoHour));

                // get the instant from the current time trimed to the hour
                endInstant = zonedHour.ToInstant();
            } // end method            

            TimeRange = new TimeRange()
            {
                UtcStart = endInstant.Minus(Duration.FromHours(hoursLookback)),
                // remove a single millisecond to avoid overlapping reports when trimming is enabled.
                UtcEnd = endInstant.Minus(Duration.FromMilliseconds(1))
            };
        } // end method
    } // end class
} // end namespace