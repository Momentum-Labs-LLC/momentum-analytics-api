using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Visits;
using Momentum.Analytics.Core.Visits.Interfaces;
using Moq;
using NodaTime;

namespace Momentum.Analytics.Core.Tests.Visits
{
    public class VisitExpirationProviderTests
    {
        private Mock<IVisitConfiguration> _visitConfiguration;
        private Mock<ILogger<VisitExpirationProvider>> _logger;

        private VisitExpirationProvider _expirationProvider;

        public VisitExpirationProviderTests()
        {
            _visitConfiguration = new Mock<IVisitConfiguration>();
            _logger = new Mock<ILogger<VisitExpirationProvider>>();

            _expirationProvider = new VisitExpirationProvider(
                _visitConfiguration.Object,
                _logger.Object);
        } // end method

        [Fact]
        public async Task CalculateVisitExpirationAsync_Sliding()
        {
            var visitWindowLength = Duration.FromHours(24);
            _visitConfiguration.Setup(x => x.IsSliding).Returns(true);
            _visitConfiguration.Setup(x => x.WindowLength).Returns(visitWindowLength);

            var activityTimestamp = SystemClock.Instance.GetCurrentInstant();

            var expiration = await _expirationProvider.GetExpirationAsync(activityTimestamp).ConfigureAwait(false);

            var expectedExpiration = activityTimestamp.Plus(visitWindowLength);

            Assert.Equal(expectedExpiration, expiration);
        } // end method

        [Fact]
        public async Task CalculateVisitExpirationAsync_Fixed_EndofDay_EST()
        {
            var estTimeZone = NodaTime.DateTimeZoneProviders.Tzdb.GetZoneOrNull("America/New_York");
            var visitWindowLength = Duration.FromHours(24);
            _visitConfiguration.Setup(x => x.IsSliding).Returns(false);
            _visitConfiguration.Setup(x => x.WindowLength).Returns(visitWindowLength);
            _visitConfiguration.Setup(x => x.FixedWindowStart).Returns(LocalTime.FromMinutesSinceMidnight(0));
            _visitConfiguration.Setup(x => x.TimeZone).Returns(estTimeZone);

            var activityTimestamp = SystemClock.Instance.GetCurrentInstant();

            var expiration = await _expirationProvider.GetExpirationAsync(activityTimestamp).ConfigureAwait(false);

            var zonedTimestamp = activityTimestamp.InZone(estTimeZone);
            var zonedWindowStart = new ZonedDateTime(
                    new LocalDateTime(
                        zonedTimestamp.Year,
                        zonedTimestamp.Month,
                        zonedTimestamp.Day,
                        0, 0),
                    estTimeZone,
                    estTimeZone.GetUtcOffset(activityTimestamp));
            var zonedExpiration = zonedWindowStart.Plus(visitWindowLength);
            var expectedExpiration = zonedExpiration.ToInstant();
            
            Assert.Equal(expectedExpiration, expiration);
        } // end method

        [Fact]
        public async Task CalculateVisitExpirationAsync_Fixed_EndofDay_EST_Midnight()
        {
            var estTimeZone = NodaTime.DateTimeZoneProviders.Tzdb.GetZoneOrNull("America/New_York");
            var visitWindowLength = Duration.FromHours(24);
            _visitConfiguration.Setup(x => x.IsSliding).Returns(false);
            _visitConfiguration.Setup(x => x.WindowLength).Returns(visitWindowLength);
            _visitConfiguration.Setup(x => x.FixedWindowStart).Returns(LocalTime.FromMinutesSinceMidnight(0));
            _visitConfiguration.Setup(x => x.TimeZone).Returns(estTimeZone);

            var now = SystemClock.Instance.GetCurrentInstant();
            var zonedNow = now.InZone(estTimeZone);
            var zonedMidnight = new ZonedDateTime(
                new LocalDateTime(
                    zonedNow.Year,
                    zonedNow.Month,
                    zonedNow.Day,
                    0, 0),
                estTimeZone,
                estTimeZone.GetUtcOffset(now));
            var midnightTimestamp = zonedMidnight.ToInstant();

            var expiration = await _expirationProvider.GetExpirationAsync(midnightTimestamp).ConfigureAwait(false);

            var zonedExpiration = zonedMidnight.Plus(visitWindowLength);
            var expectedExpiration = zonedExpiration.ToInstant();
            
            Assert.Equal(expectedExpiration, expiration);
        } // end method
    } // end class
} // end namespace