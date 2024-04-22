using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Visits;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Moq;
using NodaTime;
using NodaTime.TimeZones;
using Xunit.Sdk;

namespace Momentum.Analytics.Core.Tests.Visits
{
    public class VisitServiceTests
    {
        private Mock<IVisitWindowCalculator> _visitWindowCalculator;
        private Mock<IVisitStorage<int, ISearchResponse<Visit>>> _visitStorage;
        private Mock<IMemoryCache> _memoryCache;
        private Mock<ILogger<TestVisitService>> _logger;

        private TestVisitService _visitService;

        public VisitServiceTests()
        {
            _visitWindowCalculator = new Mock<IVisitWindowCalculator>();
            _visitStorage = new Mock<IVisitStorage<int, ISearchResponse<Visit>>>();
            _memoryCache = new Mock<IMemoryCache>();
            _logger = new Mock<ILogger<TestVisitService>>();

            _visitService = new TestVisitService(
                _visitWindowCalculator.Object,
                _visitStorage.Object,
                _memoryCache.Object,
                _logger.Object);
        } // end method
    } // end class

    public class TestVisitService : VisitService<int, ISearchResponse<Visit>, IVisitStorage<int, ISearchResponse<Visit>>>
    {
        public TestVisitService(
                IVisitWindowCalculator visitWindowCalculator, 
                IVisitStorage<int, ISearchResponse<Visit>> visitStorage, 
                IMemoryCache memoryCache, 
                ILogger<TestVisitService> logger) 
            : base(visitWindowCalculator, visitStorage, memoryCache, logger)
        {
        } // end method
    } // end class
} // end namespace