using Momentum.Analytics.Core.Interfaces;

namespace Momentum.Analytics.Visits.Lambda
{
    public interface IIdentifiedVisitTimeRangeProvider
    {
        ITimeRange TimeRange { get; }
    } // end interface
} // end namespace