using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Momentum.Analytics.Core.PII;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.Visits;
using Momentum.Analytics.Core.Visits.Interfaces;

namespace Momentum.Analytics.Processing.PageViews
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPageViewProcessor(this IServiceCollection services)
        {
            return services
                .AddTransient<IPiiService, PiiService>()
                .AddTransient<IVisitService, VisitService>();
        } // end method
    } // end class
} // end namespace