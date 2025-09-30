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
        private Mock<ILogger<VisitWindowCalculator>> _logger;

        private VisitWindowCalculator _expirationProvider;

        public VisitExpirationProviderTests()
        {
            _visitConfiguration = new Mock<IVisitConfiguration>();
            _logger = new Mock<ILogger<VisitWindowCalculator>>();

            _expirationProvider = new VisitWindowCalculator(
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

            var expiration = await _expirationProvider.GetExpirationAsync(activityTimestamp);

            var expectedExpiration = activityTimestamp.Plus(visitWindowLength);

            Assert.Equal(expectedExpiration, expiration);
        } // end method

        [Fact]
        public async Task CalculateVisitExpirationAsync_Fixed_EndofDay_EST()
        {
            var estTimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull("America/New_York");
            if(estTimeZone == null)
            {
                throw new Exception("America/New_York time zone not found");
            }

            var visitWindowLength = Duration.FromHours(24);
            _visitConfiguration.Setup(x => x.IsSliding).Returns(false);
            _visitConfiguration.Setup(x => x.WindowLength).Returns(visitWindowLength);
            _visitConfiguration.Setup(x => x.FixedWindowStart).Returns(LocalTime.FromMinutesSinceMidnight(0));
            _visitConfiguration.Setup(x => x.TimeZone).Returns(estTimeZone);

            var activityTimestamp = SystemClock.Instance.GetCurrentInstant();

            var expiration = await _expirationProvider.GetExpirationAsync(activityTimestamp);
            var zonedTimestamp = activityTimestamp.InZone(estTimeZone);

            var localDate = new LocalDate(
                        zonedTimestamp.Year, 
                        zonedTimestamp.Month, 
                        zonedTimestamp.Day);

            var zonedMidnight = zonedTimestamp.Zone.AtStartOfDay(localDate);
            var firstWindowStart = Duration.FromMinutes(0);
            var zonedWindowStart = zonedMidnight.Plus(firstWindowStart);
            
            var zonedExpiration = zonedWindowStart.Plus(visitWindowLength);
            var expectedExpiration = zonedExpiration.ToInstant();
            
            Assert.Equal(expectedExpiration, expiration);
        } // end method

        [Fact]
        public async Task CalculateVisitExpirationAsync_Fixed_EndofDay_EST_Midnight()
        {
            var estTimeZone = DateTimeZoneProviders.Tzdb.GetZoneOrNull("America/New_York");
            if(estTimeZone == null)
            {
                throw new Exception("America/New_York time zone not found");
            }

            var visitWindowLength = Duration.FromHours(24);
            _visitConfiguration.Setup(x => x.IsSliding).Returns(false);
            _visitConfiguration.Setup(x => x.WindowLength).Returns(visitWindowLength);
            _visitConfiguration.Setup(x => x.FixedWindowStart).Returns(LocalTime.FromMinutesSinceMidnight(0));
            _visitConfiguration.Setup(x => x.TimeZone).Returns(estTimeZone);

            

            var now = SystemClock.Instance.GetCurrentInstant();
            var zonedNow = now.InZone(estTimeZone);
            var localDay = new LocalDate(
                    zonedNow.Year,
                    zonedNow.Month,
                    zonedNow.Day);

            var zonedMidnight = estTimeZone.AtStartOfDay(localDay);
            // var zonedMidnight = new ZonedDateTime(
            //     new LocalDateTime(
            //         zonedNow.Year,
            //         zonedNow.Month,
            //         zonedNow.Day,
            //         0, 0),
            //     estTimeZone,
            //     estTimeZone.GetUtcOffset(now));
            var midnightTimestamp = zonedMidnight.ToInstant();

            var expiration = await _expirationProvider.GetExpirationAsync(midnightTimestamp);

            var zonedExpiration = zonedMidnight.Plus(visitWindowLength);
            var expectedExpiration = zonedExpiration.ToInstant();
            
            Assert.Equal(expectedExpiration, expiration);
        } // end method
    } // end class
} // end namespace