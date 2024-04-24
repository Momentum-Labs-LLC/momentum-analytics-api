using NodaTime;

namespace Momentum.Analytics.Core.Extensions
{
    public static class InstantExtensions
    {
        public static Instant TrimToHour(this Instant instant)
        {
            var utc = instant.InUtc();
            var hourOfDay = utc.TimeOfDay.With(TimeAdjusters.TruncateToHour);

            var diff = utc.TimeOfDay - hourOfDay;

            return instant.Minus(diff.ToDuration());
        } // end method
    } // end class
} // end namespace