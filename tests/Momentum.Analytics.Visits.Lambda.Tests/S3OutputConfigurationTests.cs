using Microsoft.Extensions.Configuration;
using Momentum.Analytics.Core.Extensions;
using Momentum.Analytics.Core.Models;
using Momentum.Analytics.Core.Visits.Interfaces;
using Moq;
using NodaTime;
using NodaTime.Extensions;

namespace Momentum.Analytics.Visits.Lambda.Tests
{
    public class S3OutputConfigurationTests
    {
        private Mock<IConfiguration> _configuration;
        private Mock<IVisitConfiguration> _visitConfiguration;
        private S3OutputConfiguration _outputConfig;

        public S3OutputConfigurationTests()
        {
            _configuration = new Mock<IConfiguration>();
            _visitConfiguration = new Mock<IVisitConfiguration>();

            Configure(S3OutputConfiguration.OUTPUT_BUCKET, S3OutputConfiguration.OUTPUT_BUCKET_DEFAULT);
            _outputConfig = new S3OutputConfiguration(
                _configuration.Object,
                _visitConfiguration.Object);

            SetupTimeZone("America/New_York");            
        } // end method

        protected void Configure(string key, string value)
        {
            var configSection = new Mock<IConfigurationSection>();
            configSection.Setup(x => x.Value).Returns(value);

            _configuration.Setup(x => x.GetSection(key)).Returns(configSection.Object);
        } // end method

        protected void SetupTimeZone(string timezoneId)
        {
            var timezone = DateTimeZoneProviders.Tzdb.GetZoneOrNull(timezoneId);
            _visitConfiguration.Setup(x => x.TimeZone).Returns(timezone);
        } // end method

        [Fact]
        public async Task BuildKeyAsync()
        {
            var hoursLookback = 24;
            var hourStart = DateTime.UtcNow.Trim(TimeSpan.FromHours(1).Ticks).ToInstant();
            
            var timeRange = new TimeRange()
            {
                UtcStart = hourStart.Minus(Duration.FromHours(24)),
                UtcEnd = hourStart
            };
            var key = await _outputConfig.BuildKeyAsync(timeRange).ConfigureAwait(false);

            var timeZone = _visitConfiguration.Object.TimeZone;
            var prefix = hourStart.InZone(timeZone).ToString("yyyyMM", null);
            var start = timeRange.UtcStart.InZone(timeZone).ToString("yyyyMMddHH", null);
            var end = timeRange.UtcEnd.InZone(timeZone).ToString("yyyyMMddHH", null);
            var expected = $"{prefix}/{start}_{end}.csv";

            Assert.Equal(expected, key);
        } // end method
    } // end class
} // end namespace