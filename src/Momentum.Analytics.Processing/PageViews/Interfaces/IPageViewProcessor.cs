using Momentum.Analytics.Core.PageViews.Models;

namespace Momentum.Analytics.Processing.PageViews.Interfaces
{
    public interface IPageViewProcessor
    {
        Task ProcessAsync(PageView pageView, CancellationToken token = default);
    } // end interface
} // end namespace