using Microsoft.Extensions.Configuration;
using Momentum.Analytics.Core;
using Momentum.Analytics.Core.Extensions;
using Momentum.Analytics.Core.Interfaces;
using Moq;
using NodaTime;

namespace Momentum.Analytics.Export.Lambda.Tests;

public class TimeRangeProviderTests
{

    private Mock<IConfiguration> _configuration;
    private IDateTimeZoneProvider _timezoneProvider;
    private IClockService _clockService;
    private TimeRangeProvider? _timeRangeProvider;

    public TimeRangeProviderTests()
    {
        _configuration = new Mock<IConfiguration>();
        _timezoneProvider = new NodaTime.TimeZones.DateTimeZoneCache(NodaTime.TimeZones.TzdbDateTimeZoneSource.Default);
        _clockService = new ClockService(SystemClock.Instance);
    } // end method

    private void SetupConfigurationValue(string key, string value)
    {
        var configSection = new Mock<IConfigurationSection>();
        configSection.Setup(x => x.Value).Returns(value);
        _configuration.Setup(x => x.GetSection(key)).Returns(configSection.Object);
    } // end method

    [Theory]
    [InlineData(24)]
    [InlineData(12)]
    public void GetTimeRange(int hoursLookback)
    {
        SetupConfigurationValue("TIMEZONE", "America/New_York");
        SetupConfigurationValue("TRIM_TO_HOUR", "true");
        SetupConfigurationValue("HOURS_LOOKBACK", hoursLookback.ToString());
        
        _timeRangeProvider = new TimeRangeProvider(_configuration.Object, _timezoneProvider, _clockService);

        var timeRange = _timeRangeProvider.TimeRange;

        var now = _clockService.Now;
        var endHour = now.TrimToHour();
        var expectedStart = endHour.Minus(Duration.FromHours(hoursLookback));
        var expectedEnd = endHour.Minus(Duration.FromMilliseconds(1));

        Assert.NotNull(timeRange);
        Assert.Equal(expectedStart, timeRange.UtcStart);
        Assert.Equal(expectedEnd, timeRange.UtcEnd);
    }
} // end class