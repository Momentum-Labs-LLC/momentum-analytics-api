using Momentum.Analytics.Core.Interfaces;

namespace Momentum.Analytics.Visits.Lambda
{
    public interface IIdentifiedVisitTimeRangeProvider
    {
        bool TrimToHour { get; }
        ITimeRange TimeRange { get; }
    } // end interface
} // end namespace