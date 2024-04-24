using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Models;
using Momentum.Analytics.Core.PageViews.Models;
using Momentum.Analytics.Core.Visits;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Core.Visits.Models;
using Moq;
using NodaTime;
using NodaTime.Extensions;
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
            _logger = new Mock<ILogger<TestVisitService>>();

            _visitService = new TestVisitService(
                _visitWindowCalculator.Object,
                _visitStorage.Object,
                _logger.Object);
        } // end method

        [Fact]
        public async Task GetByActivityAsync_NotFound()
        {
            IUserActivity userActivity = new PageView()
            {
                CookieId = Guid.NewGuid(),
                UtcTimestamp = DateTime.UtcNow.ToInstant()
            };

            _visitStorage.Setup(x => x.GetLatestAysnc(userActivity.CookieId, userActivity.UtcTimestamp, It.IsAny<CancellationToken>()))
                .ReturnsAsync((Visit)null);

            var visit = await _visitService.GetByActivityAsync(userActivity).ConfigureAwait(false);

            Assert.Null(visit);

            _visitStorage.Verify(x => x.GetLatestAysnc(userActivity.CookieId, userActivity.UtcTimestamp, It.IsAny<CancellationToken>()), Times.Once);
        } // end method

        [Fact]
        public async Task GetByActivityAsync_OutsideRange()
        {
            IUserActivity userActivity = new PageView()
            {
                CookieId = Guid.NewGuid(),
                UtcTimestamp = DateTime.UtcNow.ToInstant()
            };

            _visitStorage.Setup(x => x.GetLatestAysnc(userActivity.CookieId, userActivity.UtcTimestamp, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Visit()
                {
                    UtcStart = DateTime.UtcNow.ToInstant().Minus(Duration.FromHours(48)),
                    UtcExpiration = DateTime.UtcNow.ToInstant().Minus(Duration.FromHours(24))
                });

            var visit = await _visitService.GetByActivityAsync(userActivity).ConfigureAwait(false);

            Assert.Null(visit);

            _visitStorage.Verify(x => x.GetLatestAysnc(userActivity.CookieId, userActivity.UtcTimestamp, It.IsAny<CancellationToken>()), Times.Once);
        } // end method

        [Fact]
        public async Task GetByActivityAsync_InRange()
        {
            IUserActivity userActivity = new PageView()
            {
                CookieId = Guid.NewGuid(),
                UtcTimestamp = DateTime.UtcNow.ToInstant()
            };

            _visitStorage.Setup(x => x.GetLatestAysnc(userActivity.CookieId, userActivity.UtcTimestamp, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Visit()
                {
                    UtcStart = DateTime.UtcNow.ToInstant().Minus(Duration.FromHours(12)),
                    UtcExpiration = DateTime.UtcNow.ToInstant().Plus(Duration.FromHours(12))
                });

            var visit = await _visitService.GetByActivityAsync(userActivity).ConfigureAwait(false);

            Assert.NotNull(visit);

            _visitStorage.Verify(x => x.GetLatestAysnc(userActivity.CookieId, userActivity.UtcTimestamp, It.IsAny<CancellationToken>()), Times.Once);
        } // end method
    } // end class

    public class TestVisitService : VisitService<int, ISearchResponse<Visit>, IVisitStorage<int, ISearchResponse<Visit>>>
    {
        public TestVisitService(
                IVisitWindowCalculator visitWindowCalculator, 
                IVisitStorage<int, ISearchResponse<Visit>> visitStorage, 
                ILogger<TestVisitService> logger) 
            : base(visitWindowCalculator, visitStorage, logger)
        {
        } // end method
    } // end class
} // end namespace