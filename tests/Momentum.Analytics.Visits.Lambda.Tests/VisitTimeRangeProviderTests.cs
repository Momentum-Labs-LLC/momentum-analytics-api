using Microsoft.Extensions.Configuration;
using Momentum.Analytics.Core;
using Momentum.Analytics.Core.Extensions;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Visits.Interfaces;
using Moq;

namespace Momentum.Analytics.Visits.Lambda.Tests
{
    public class VisitTimeRangeProviderTests
    {
        protected readonly Mock<IConfiguration> _configuration;
        protected readonly Mock<IVisitConfiguration> _visitConfiguration;
        protected readonly IClockService _clockService;

        private IdentifiedVisitTimeRangeProvider _timeRangeProvider;

        public VisitTimeRangeProviderTests()
        {
            _configuration = new Mock<IConfiguration>();
            _visitConfiguration = new Mock<IVisitConfiguration>();
            _clockService = new ClockService();

            SetupTimezone("America/New_York");
        } // end method

        protected void SetupTimezone(string timezoneId)
        {
            var timeZone = NodaTime.DateTimeZoneProviders.Tzdb.GetZoneOrNull(timezoneId);
            _visitConfiguration.Setup(x => x.TimeZone).Returns(timeZone);
        } // end method

        protected void SetupHoursLookback(int? lookbackHours = null)
        {
            var configSection = new Mock<IConfigurationSection>();
            configSection.Setup(x => x.Value).Returns(lookbackHours.HasValue ? lookbackHours.Value.ToString() : null);

            _configuration.Setup(x => x.GetSection(IdentifiedVisitTimeRangeProvider.HOURS_LOOKBACK))
                .Returns(configSection.Object);
        } // end method

        [Fact]
        public async Task GetAsync()
        {
            SetupHoursLookback();
            _timeRangeProvider = new IdentifiedVisitTimeRangeProvider(_configuration.Object, _visitConfiguration.Object, _clockService);
            
            var utcHour = DateTime.UtcNow.Trim(TimeSpan.FromHours(1).Ticks);
            var utcStart = utcHour.AddHours(-24);

            var timeRange = _timeRangeProvider.TimeRange;

            Assert.NotNull(timeRange);
            // TODO: Fix asserts
            // Assert.Equal(utcStart, timeRange.UtcStart);
            // Assert.Equal(utcHour, timeRange.UtcEnd);
        } // end method
    } // end class
} // end namespace