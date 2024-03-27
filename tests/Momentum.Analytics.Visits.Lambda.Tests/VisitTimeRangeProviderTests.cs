using Microsoft.Extensions.Configuration;
using Momentum.Analytics.Core.Extensions;
using Moq;

namespace Momentum.Analytics.Visits.Lambda.Tests
{
    public class VisitTimeRangeProviderTests
    {
        protected readonly Mock<IConfiguration> _configuration;

        private VisitTimeRangeProvider _timeRangeProvider;

        public VisitTimeRangeProviderTests()
        {
            _configuration = new Mock<IConfiguration>();
        } // end method

        protected void SetupHoursLookback(int? lookbackHours = null)
        {
            var configSection = new Mock<IConfigurationSection>();
            configSection.Setup(x => x.Value).Returns(lookbackHours.HasValue ? lookbackHours.Value.ToString() : null);

            _configuration.Setup(x => x.GetSection(VisitTimeRangeProvider.HOURS_LOOKBACK))
                .Returns(configSection.Object);
        } // end method

        [Fact]
        public async Task GetAsync()
        {
            SetupHoursLookback();
            _timeRangeProvider = new VisitTimeRangeProvider(_configuration.Object);
            
            var utcHour = DateTime.UtcNow.Trim(TimeSpan.FromHours(1).Ticks);
            var utcStart = utcHour.AddHours(-24);

            var timeRange = _timeRangeProvider.TimeRange;

            Assert.NotNull(timeRange);
            Assert.Equal(utcStart, timeRange.UtcStart);
            Assert.Equal(utcHour, timeRange.UtcEnd);
        } // end method
    } // end class
} // end namespace