using NodaTime;

namespace Momentum.Analytics.Core.Visits.Interfaces
{
    public interface IVisitConfiguration
    {
        /// <summary>
        /// Gets if this is a sliding visit window.
        /// </summary>
        public bool IsSliding { get; }

        /// <summary>
        /// Gets the window's length of time.
        /// </summary>
        Duration WindowLength { get; }

        /// <summary>
        /// Gets the time when the fixed window starts. Applies when 
        /// </summary>
        LocalTime? FixedWindowStart { get; }

        /// <summary>
        /// Gets the timezone for visits that have a fixed window start.
        /// </summary>
        DateTimeZone? TimeZone { get; }

        /// <summary>
        /// Gets the time to cache locally.
        /// </summary>
        Duration LocalCacheExpiration { get; }
    } // end interface
} // end namespace