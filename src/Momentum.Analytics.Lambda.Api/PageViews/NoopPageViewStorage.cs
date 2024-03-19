using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.PageViews;
using Momentum.Analytics.Core.PageViews.Interfaces;
using Momentum.Analytics.Core.PageViews.Models;

namespace Momentum.Analytics.Lambda.Api.PageViews
{
    public class NoopPageViewStorage : IPageViewStorage
    {
        public Task InsertAsync(PageView pageView, CancellationToken token = default)
        {
            return Task.CompletedTask;
        } // end method
    } // end class

    public static class NoopPageViewServiceExtensions 
    {
        public static IServiceCollection AddNoopPageViewService(this IServiceCollection service)
        {
            return service
                .AddTransient<IPageViewStorage, NoopPageViewStorage>()
                .AddTransient<IPageViewService, PageViewService>();
        } // end method
    } // end class
} // end namespace