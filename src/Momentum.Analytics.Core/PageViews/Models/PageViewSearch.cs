using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Models;
using Momentum.Analytics.Core.PageViews.Interfaces;

namespace Momentum.Analytics.Core.PageViews.Models
{
    public class PageViewSearch : SearchRequest, IPageViewSearch
    {
        public Guid? CookieId { get; set; }
        public ITimeRange? TimeRange { get; set; }
    } // end class
} // end namespace