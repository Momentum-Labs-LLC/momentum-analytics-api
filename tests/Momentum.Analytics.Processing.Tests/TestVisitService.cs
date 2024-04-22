using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Visits;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Core.Visits.Models;

namespace Momentum.Analytics.Processing.Tests
{
    public interface ITestVisitStorage : IVisitStorage<int, ISearchResponse<Visit>> {} // end method
    public interface ITestVisitService : IVisitService<int, ISearchResponse<Visit>> {} // end method
    public class TestVisitService :
        VisitService<int, ISearchResponse<Visit>, ITestVisitStorage>,
        ITestVisitService
    {
        public TestVisitService(
                IVisitWindowCalculator visitExpirationProvider,
                ITestVisitStorage visitStorage, 
                IMemoryCache memoryCache, 
                ILogger<TestVisitService> logger) 
            : base(visitExpirationProvider, visitStorage, memoryCache, logger)
        {
        } // end method
    } // end class
} // end namespace