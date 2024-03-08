using Momentum.Analytics.Core.PageViews.Models;

namespace Momentum.Analytics.Core.PageViews.Interfaces
{
    public interface IPageViewService
    {
        Task RecordAsync(PageView pageView, CancellationToken token = default);
        Task<bool> IsNewVisitAsync(PageView pageView, CancellationToken token = default);
        Task<IEnumerable<PageView>> GetByCookieAsync(Guid cookieId, CancellationToken token = default);
    } // end interface
} // end namespace