using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.PageViews.V2.Interfaces;

namespace Momentum.Analytics.Core.PageViews.V2
{
    public interface IPageViewV2Service
    {
        Task RecordAsync(PageView pageView, CancellationToken token = default);
    }
    public class PageViewService : IPageViewV2Service
    {
        protected readonly IPageViewV2Storage _storage;
        protected readonly ILogger _logger;

        public PageViewService(
            IPageViewV2Storage storage,
            ILogger<PageViewService> logger)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        public virtual async Task RecordAsync(PageView pageView, CancellationToken token = default)
        {
            await _storage.InsertAsync(pageView, token).ConfigureAwait(false);
        } // end method
    } // end class
} // end namespace