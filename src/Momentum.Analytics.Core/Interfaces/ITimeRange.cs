namespace Momentum.Analytics.Core.Interfaces
{
    public interface ITimeRange
    {
        DateTime? UtcStart { get; }
        DateTime? UtcEnd { get; }
    } // end interface
} // end namespace