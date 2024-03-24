using Momentum.Analytics.Core.Interfaces;

namespace Momentum.Analytics.Visits.Lambda
{
    public interface IVisitTimeRangeProvider
    {
        ITimeRange TimeRange { get; }
    } // end interface
} // end namespace