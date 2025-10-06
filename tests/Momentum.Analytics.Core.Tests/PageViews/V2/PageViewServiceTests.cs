using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.PageViews.V2;
using Momentum.Analytics.Core.PageViews.V2.Interfaces;
using Momentum.Analytics.Core.PII.Interfaces;
using Momentum.Analytics.Core.PII.Models;
using Moq;

namespace Momentum.Analytics.Core.Tests.PageViews.V2;

public class PageViewServiceTests
{
    private Mock<IPageViewV2Storage> _storage;
    private Mock<IPiiService> _piiService;
    private Mock<ILogger<PageViewService>> _logger;
    private PageViewService _service;
    private IClockService _clockService;

    public PageViewServiceTests()
    {
        _clockService = new ClockService();
        _storage = new Mock<IPageViewV2Storage>();
        _piiService = new Mock<IPiiService>();
        _logger = new Mock<ILogger<PageViewService>>();
        _service = new PageViewService(_storage.Object, _piiService.Object, _logger.Object);
    } // end method

    [Fact]
    public async Task RecordAsync_NoAppointmentId()
    {
        var pageView = new PageView()
        {
            CookieId = Guid.NewGuid(),
            VisitId = Ulid.NewUlid(),
            UtcTimestamp = _clockService.Now,
            Url = "https://test.com",
            Referer = "https://test.com"
        };

        await _service.RecordAsync(pageView);

        _storage.Verify(x => x.InsertAsync(pageView, It.IsAny<CancellationToken>()), Times.Once);
        _piiService.Verify(x => x.RecordAsync(It.IsAny<CollectedPii>(), It.IsAny<CancellationToken>()), Times.Never);
    } // end method

    [Theory]
    [InlineData("https://test.com?yourAppointmentId=12345")]
    [InlineData("https://test.com?appointmentId=12345")]
    [InlineData("https://test.com?appointmentId=12345&weirdKey&weirdKey2=")]
    public async Task RecordAsync_WithAppointmentId(string url)
    {
        var pageView = new PageView()
        {
            CookieId = Guid.NewGuid(),
            VisitId = Ulid.NewUlid(),
            UtcTimestamp = _clockService.Now,
            Url = url,
            Referer = "https://test.com"
        };

        await _service.RecordAsync(pageView);

        _storage.Verify(x => x.InsertAsync(pageView, It.IsAny<CancellationToken>()), Times.Once);
        _piiService.Verify(x => x.RecordAsync(It.Is<CollectedPii>(x => x.Pii!.Value == "12345" && x.Pii!.PiiType == PiiTypeEnum.AppointmentId), It.IsAny<CancellationToken>()), Times.Once);
    } // end method

    [Theory]
    [InlineData("https://test.com?doubleKey=123&doubleKey=321")]
    public async Task RecordAsync_WeirdQueryStrings(string url)
    {
        var pageView = new PageView()
        {
            CookieId = Guid.NewGuid(),
            VisitId = Ulid.NewUlid(),
            UtcTimestamp = _clockService.Now,
            Url = url,
            Referer = "https://test.com"
        };

        await _service.RecordAsync(pageView);

        _storage.Verify(x => x.InsertAsync(pageView, It.IsAny<CancellationToken>()), Times.Once);
        _piiService.Verify(x => x.RecordAsync(It.IsAny<CollectedPii>(), It.IsAny<CancellationToken>()), Times.Never);
    }
} // end class