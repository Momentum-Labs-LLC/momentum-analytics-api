using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Momentum.Analytics.Core.Interfaces;
using NodaTime;
using NodaTime.TimeZones;

namespace Momentum.Analytics.Core
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddNodaTime(this IServiceCollection services)
        {
            return services
                .AddSingleton<IDateTimeZoneSource>(NodaTime.TimeZones.TzdbDateTimeZoneSource.Default)
                .AddSingleton<IDateTimeZoneProvider, NodaTime.TimeZones.DateTimeZoneCache>()
                .AddSingleton<IClock>(NodaTime.SystemClock.Instance)
                .AddSingleton<IClockService, ClockService>();
        } // end methoid
    } // end class
} // end namespace