using Momentum.Analytics.Core.PageViews.V2;
using Momentum.Analytics.Lambda.Api.Cookies;
using NodaTime;

namespace Momentum.Analytics.Lambda.Api.PageViews.V2
{
    public static class PageViewExtensions
    {
        public static PageView ToDomain(this PageViewV2ViewModel viewModel, Cookie cookie, Instant timestamp)
        {
            return new PageView()
            {
                CookieId = cookie.Id,
                VisitId = cookie.VisitId,
                UtcTimestamp = timestamp,
                Url = viewModel.Url,
                Referer = viewModel.Referer
            };
        }
    }
}