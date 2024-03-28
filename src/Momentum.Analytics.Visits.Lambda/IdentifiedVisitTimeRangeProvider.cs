using Microsoft.Extensions.Configuration;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Models;
using Momentum.Analytics.Core.Visits.Interfaces;
using NodaTime;

namespace Momentum.Analytics.Visits.Lambda
{
    public class IdentifiedVisitTimeRangeProvider : IIdentifiedVisitTimeRangeProvider
    {
        public const string HOURS_LOOKBACK = "HOURS_LOOKBACK";
        public const int HOURS_LOOKBACK_DEFAULT = 24;
        public ITimeRange TimeRange { get; protected set; }

        public IdentifiedVisitTimeRangeProvider(IConfiguration configuration, IVisitConfiguration visitConfiguration, IClockService clockService)
        {
            var now = clockService.Now;
            var hoursLookback = configuration.GetValue<int>(HOURS_LOOKBACK, HOURS_LOOKBACK_DEFAULT);

            var zoned = now.InUtc();
            if(visitConfiguration.TimeZone != null)
            {
                zoned = now.InZone(visitConfiguration.TimeZone);
            } // end if

            // get the time of day trimed to hour
            var specificHour = zoned.TimeOfDay.With(TimeAdjusters.TruncateToHour);

            // get the zoned time trimed to the hour
            var zonedHour = zoned.Minus(Duration.FromNanoseconds((zoned.TimeOfDay - specificHour).Nanoseconds));

            // get the instant from the current time trimed to the hour
            var endInstant = zonedHour.ToInstant();

            TimeRange = new TimeRange()
            {
                UtcStart = endInstant.Minus(Duration.FromHours(hoursLookback)),
                UtcEnd = endInstant
            };
        } // end method
    } // end class
} // end namespace