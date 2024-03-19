using Momentum.Analytics.Core.PageViews.Models;

namespace Momentum.Analytics.Core.PageViews.Interfaces
{
    public interface IPageViewService
    {
        Task RecordAsync(PageView pageView, CancellationToken token = default);
    } // end interface
} // end namespace