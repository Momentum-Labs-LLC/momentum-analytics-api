namespace Momentum.Analytics.Core.Visits.Interfaces
{
    public interface IVisitConfiguration
    {
        /// <summary>
        /// Gets or sets if the window for a visit is fixed or sliding.
        /// </summary>
        bool IsSliding { get; }

        /// <summary>
        /// Gets or sets the window's length of time.
        /// </summary>
        TimeSpan WindowLength { get; }

        /// <summary>
        /// Gets the time to cache locally.
        /// </summary>
        TimeSpan LocalCacheExpiration { get; }
    } // end interface
} // end namespace