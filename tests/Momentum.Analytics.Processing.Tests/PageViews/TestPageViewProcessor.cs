using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.Processing.Cookies;
using Momentum.Analytics.Processing.PageViews;
using Momentum.Analytics.Processing.PageViews.Interfaces;

namespace Momentum.Analytics.Processing.Tests.PageViews
{
    public interface ITestPageViewProcessor : IPageViewProcessor {} // end interface
    public class TestPageViewProcessor :
        PageViewProcessor<int, ISearchResponse<Visit>, ITestVisitService>,
        ITestPageViewProcessor
    {
        public TestPageViewProcessor(
                ITestVisitService visitService, 
                ISharedCookieConfiguration sharedCookieConfiguration,
                IClockService clockService,
                ILogger<TestPageViewProcessor> logger) 
            : base(visitService, sharedCookieConfiguration, clockService, logger)
        {
        } // end method
    } // end class
} // end namespace