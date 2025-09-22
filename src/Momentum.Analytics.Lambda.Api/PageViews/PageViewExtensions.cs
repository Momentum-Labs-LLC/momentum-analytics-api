using Momentum.Analytics.Core.PageViews.Models;
using Momentum.Analytics.Lambda.Api.Cookies;
using Momentum.Analytics.Lambda.Api.PageViews.ViewModels;
using NodaTime;

namespace Momentum.Analytics.Lambda.Api.PageViews
{
    public static class PageViewExtensions
    {
        public static PageView ToDomain(this PageViewViewModel viewModel, Cookie cookie, Instant timestamp)
        {
            return new PageView()
            {
                CookieId = cookie.Id,
                Referer = viewModel.Referer,
                UtmParameters = viewModel.UtmParameters?
                    .Where(x => PageViewConstants.UTM_PARAMETERS.ContainsKey(x.Key))
                    .Select(x => new UrchinTrackingParameter()
                    {
                        Parameter = PageViewConstants.UTM_PARAMETERS[x.Key],
                        Value = x.Value
                    })
                    .ToList() ?? null,
                Domain = viewModel.Domain,
                Path = viewModel.Path,
                FunnelStep = viewModel.FunnelStep,
                UtcTimestamp = timestamp
            };
        } // end method
    } // end class
} // end namespace