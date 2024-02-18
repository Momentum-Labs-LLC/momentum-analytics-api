using Momentum.Pixel.Core.PageViews.Models;

namespace Momentum.Pixel.Core.PageViews.Interfaces
{
    public interface IPageViewService
    {
        Task RecordAsync(PageView pageView, CancellationToken token = default);
    } // end interface
} // end namespace