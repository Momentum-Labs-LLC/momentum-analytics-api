namespace Momentum.Analytics.Core.PageViews.V2.Interfaces
{
    public interface IPageViewV2Storage
    {
        Task InsertAsync(PageView pageView, CancellationToken token = default);
    } // end interface
} // end namespace