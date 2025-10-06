using Microsoft.Extensions.Configuration;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Models;
using NodaTime;

namespace Momentum.Analytics.Export.Lambda;

public interface ITimeRangeProvider
{
    ITimeRange TimeRange { get; }
} // end interface

public class TimeRangeProvider : ITimeRangeProvider
{
    public const string TIMEZONE = "TIMEZONE";
    public const string TIMEZONE_DEFAULT = "America/New_York";
    public const string TIMEZONE_UTC = "Etc/UTC";
    public const string TRIM_TO_HOUR = "TRIM_TO_HOUR";
    public const bool TRIM_TO_HOUR_DEFAULT = true;
    public const string HOURS_LOOKBACK = "HOURS_LOOKBACK";
    public const int HOURS_LOOKBACK_DEFAULT = 24;

    public ITimeRange TimeRange { get; protected set; }

    public TimeRangeProvider(IConfiguration configuration, IDateTimeZoneProvider timezoneProviders, IClockService clockService)
    {
        var now = clockService.Now;
        var hoursLookback = configuration.GetValue<int>(HOURS_LOOKBACK, HOURS_LOOKBACK_DEFAULT);
        var trimToHour = configuration.GetValue<bool>(TRIM_TO_HOUR, TRIM_TO_HOUR_DEFAULT);
        var timezoneId = configuration.GetValue<string>(TIMEZONE, TIMEZONE_DEFAULT);
        DateTimeZone? timeZone = timezoneProviders.GetZoneOrNull(TIMEZONE_UTC);
        
        if(!string.IsNullOrWhiteSpace(timezoneId))
        {
            timeZone = timezoneProviders.GetZoneOrNull(timezoneId) ?? DateTimeZone.Utc;
        }

        var zoned = now.InZone(timeZone!);

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