using Microsoft.Extensions.Configuration;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Models;

namespace Momentum.Analytics.Visits.Lambda
{
    public class VisitTimeRangeProvider : IVisitTimeRangeProvider
    {
        public const string HOURS_LOOKBACK = "HOURS_LOOKBACK";
        public const int HOURS_LOOKBACK_DEFAULT = 24;
        public ITimeRange TimeRange { get; protected set; }

        public VisitTimeRangeProvider(IConfiguration configuration)
        {
            var utcHour = DateTime.UtcNow.Trim(TimeSpan.FromHours(1).Ticks);
            
            var hoursLookback = configuration.GetValue<int>(HOURS_LOOKBACK, HOURS_LOOKBACK_DEFAULT);

            TimeRange = new TimeRange()
            {
                UtcStart = utcHour.AddHours(hoursLookback * -1),
                UtcEnd = utcHour
            };
        } // end method
    } // end class

    public static class DateTimeExtensions
    {
        public static DateTime Trim(this DateTime dateTime, long intervalTicks)
        {
            return new DateTime(dateTime.Ticks - (dateTime.Ticks % intervalTicks), dateTime.Kind);
        } // end method
    } // end class
} // end namespace