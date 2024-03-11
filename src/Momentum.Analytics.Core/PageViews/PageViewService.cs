using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.PageViews.Interfaces;
using Momentum.Analytics.Core.PageViews.Models;

namespace Momentum.Analytics.Core.PageViews
{
    public class PageViewService : IPageViewService
    {
        protected readonly IPageViewStorage _storage;
        protected readonly IMemoryCache _memoryCache;
        protected readonly ILogger _logger;

        public PageViewService(
            IPageViewStorage storage,
            IMemoryCache memoryCache,
            ILogger<PageViewService> logger)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method
        
        public virtual async Task<ISearchResponse<PageView>> SearchAsync(IPageViewSearch searchRequest, CancellationToken token = default)
        {
            throw new NotImplementedException();
        } // end method

        public virtual async Task<bool> IsNewVisitAsync(PageView pageView, CancellationToken token = default)
        {
            var result = false;

            var searchRequest = new PageViewSearch() 
            { 
                CookieId = pageView.CookieId
            };
            var otherPageViews = await SearchAsync(searchRequest, token).ConfigureAwait(false);
            if(otherPageViews != null 
                && otherPageViews.Data != null
                && otherPageViews.Data.Any(x => x.RequestId != pageView.RequestId))
            {
                // TODO: find out if the page view 
            }
            else
            {
                result = true;
            } // end if

            return result;
        } // end method

        public virtual async Task RecordAsync(PageView pageView, CancellationToken token = default)
        {
            await _storage.InsertAsync(pageView, token).ConfigureAwait(false);

        } // end method
    } // end class
} // end namespace