using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.PageViews.Interfaces;
using Momentum.Analytics.Core.PageViews.Models;

namespace Momentum.Analytics.Core.PageViews
{
    public class PageViewService : IPageViewService
    {
        protected readonly IPageViewStorage _storage;
        protected readonly ILogger _logger;

        public PageViewService(
            IPageViewStorage storage,
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