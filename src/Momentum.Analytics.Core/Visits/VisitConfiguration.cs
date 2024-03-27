using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Momentum.Analytics.Core.Visits.Interfaces;
using NodaTime;

namespace Momentum.Analytics.Core.Visits
{
    public class VisitConfiguration : IVisitConfiguration
    {
        public bool IsSliding { get; protected set; }
        public Duration WindowLength { get; protected set; }
        public LocalTime? FixedWindowStart { get; protected set; }
        public DateTimeZone? TimeZone { get; protected set; }
        public Duration LocalCacheExpiration { get; protected set; }

        public VisitConfiguration(IConfiguration configuration, IDateTimeZoneProvider timezoneProviders)
        {
            IsSliding = configuration.GetValue<bool>(VisitConstants.IS_SLIDING, VisitConstants.IS_SLIDING_DEFAULT);

            var windowLengthMinutes = configuration.GetValue<int>(VisitConstants.WINDOW_LENGTH, VisitConstants.WINDOW_LENGTH_DEFAULT);
            WindowLength = Duration.FromMinutes(windowLengthMinutes);

            var fixedWindowStartMinutes = configuration.GetValue<int>(VisitConstants.FIXED_VISIT_WINDOW_START, VisitConstants.FIXED_VISIT_WINDOW_START_DEFAULT);
            if(fixedWindowStartMinutes >= 0)
            {
                FixedWindowStart = LocalTime.FromMinutesSinceMidnight(fixedWindowStartMinutes);
            } // end if

            var timezoneId = configuration.GetValue<string>(VisitConstants.TIMEZONE, VisitConstants.TIMEZONE_DEFAULT);
            if(!string.IsNullOrWhiteSpace(timezoneId))
            {
                TimeZone = timezoneProviders.GetZoneOrNull(timezoneId);
            } // end if

            var localCacheSeconds = configuration.GetValue<int>(VisitConstants.LOCAL_CACHE_EXPIRATION, VisitConstants.LOCAL_CACHE_EXPIRATION_DEFAULT);
            LocalCacheExpiration = Duration.FromSeconds(localCacheSeconds);
        } // end method
    } // end class
} // end namespace