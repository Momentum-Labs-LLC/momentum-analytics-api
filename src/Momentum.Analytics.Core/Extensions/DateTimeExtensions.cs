namespace Momentum.Analytics.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static DateTime Trim(this DateTime dateTime, long intervalTicks)
        {
            return new DateTime(dateTime.Ticks - (dateTime.Ticks % intervalTicks), dateTime.Kind);
        } // end method
    } // end class
} // end namespace