using Momentum.Analytics.Core.Interfaces;

namespace Momentum.Analytics.Core.PageViews.Interfaces
{
    public interface IPageViewSearch : ISearchRequest
    {
        public Guid? CookieId { get; set; }
        public ITimeRange? TimeRange { get; set; }
    } // end interface
} // end namespace