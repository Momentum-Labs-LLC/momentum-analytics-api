using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.Lambda.Api.Cookies;
using Momentum.Analytics.Lambda.Api.PageViews.V2;
using NodaTime;

namespace Momentum.Analytics.Lambda.Api.Tests.PageViews.V2;

public class PageViewExtensionsTests
{
    [Fact]
    public void PageViewV2ViewModel_ToDomain()
    {
        var viewModel = new PageViewV2ViewModel()
        {
            Url = "https://www.example.com",
            Referer = "https://www.example.com"
        };

        var cookie = new Cookie()
        {
            Id = Guid.NewGuid(),
            VisitId = Ulid.NewUlid(),
            VisitExpiration = Instant.FromUnixTimeMilliseconds(1000),
            CollectedPii = PiiTypeEnum.None,
            MaxFunnelStep = 0,
            UserId = null
        };

        var now = Instant.FromUnixTimeMilliseconds(2000);

        var domainModel = viewModel.ToDomain(cookie, now);

        Assert.Equal(cookie.Id, domainModel.CookieId);
        Assert.Equal(cookie.VisitId, domainModel.VisitId);
        Assert.Equal(now, domainModel.UtcTimestamp);
        Assert.Equal(viewModel.Url, domainModel.Url);
        Assert.Equal(viewModel.Referer, domainModel.Referer);
    }
}