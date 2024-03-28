using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
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
                IVisitExpirationProvider visitExpirationProvider,
                ILogger<TestCollectedPiiProcessor> logger) 
            : base(visitService, clockService, visitExpirationProvider, logger)
        {
        } // end method
    } // end class
} // end namespace