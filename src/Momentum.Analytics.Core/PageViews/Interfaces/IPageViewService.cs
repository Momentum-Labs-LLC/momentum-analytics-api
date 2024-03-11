using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.PageViews.Models;

namespace Momentum.Analytics.Core.PageViews.Interfaces
{
    public interface IPageViewService
    {
        Task RecordAsync(PageView pageView, CancellationToken token = default);
        Task<bool> IsNewVisitAsync(PageView pageView, CancellationToken token = default);
        Task<ISearchResponse<PageView>> SearchAsync(IPageViewSearch searchRequest, CancellationToken token = default);
    } // end interface
} // end namespace