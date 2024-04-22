using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Momentum.Analytics.Processing.Cookies;
using Momentum.Analytics.Processing.Pii;
using Momentum.Analytics.Processing.Pii.Interfaces;

namespace Momentum.Analytics.Processing.Tests.Pii
{
    public interface ITestCollectedPiiProcessor : ICollectedPiiProcessor {} // end interface
    public class TestCollectedPiiProcessor :
        CollectedPiiProcessor<int, ISearchResponse<Visit>, ITestVisitService>,
        ITestCollectedPiiProcessor
    {
        public TestCollectedPiiProcessor(
                ITestVisitService visitService, 
                IClockService clockService,
                IVisitWindowCalculator visitWindowCalculator,
                IPiiService piiSerivce,
                ISharedCookieConfiguration sharedCookieConfiguration,
                ILogger<TestCollectedPiiProcessor> logger) 
            : base(visitService, clockService, visitWindowCalculator, piiSerivce, sharedCookieConfiguration, logger)
        {
        } // end method
    } // end class
} // end namespace