using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.Lambda.Api.Cookies;
using NodaTime;

namespace Momentum.Analytics.Lambda.Api.Tests.Cookies;

public class CookieExtensionsTests
{
    [Theory]
    [InlineData("")]
    [InlineData((string?)null)]    
    public void ToCookieModel_NoInput(string? cookieValue)
    {
        var visitExpirationDefault = Instant.FromUnixTimeMilliseconds(1000);
        var cookieModel = cookieValue.ToCookieModel(visitExpirationDefault);

        Assert.True(cookieModel.Id != Guid.Empty);
        Assert.True(cookieModel.VisitId != Ulid.Empty);
        Assert.Equal(visitExpirationDefault, cookieModel.VisitExpiration);
        Assert.Equal(PiiTypeEnum.None, cookieModel.CollectedPii);
        Assert.Equal(0, cookieModel.MaxFunnelStep);
        Assert.Null(cookieModel.UserId);
    }

    [Fact]
    public void ToCookieModel_OldCookie()
    {
        var cookieId = Guid.NewGuid();

        var visitExpiration = Instant.FromUnixTimeMilliseconds(1000);
        var cookieValue = $"{CookieConstants.COOKIE_ID}={cookieId},{CookieConstants.VISIT_EXPIRATION}={visitExpiration.ToUnixTimeMilliseconds()},{CookieConstants.PII_BITMAP}={0},{CookieConstants.MAX_FUNNEL_STEP}={0},{CookieConstants.USER_ID}=";

        var cookie = cookieValue.ToCookieModel(visitExpiration);

        Assert.Equal(cookieId, cookie.Id);
        Assert.Equal(visitExpiration, cookie.VisitExpiration);
        Assert.Equal(PiiTypeEnum.None, cookie.CollectedPii);
        Assert.Equal(0, cookie.MaxFunnelStep);
        Assert.Null(cookie.UserId);

        Assert.NotEqual(cookieValue, cookie.ToHeaderValue().ToString());
    }

    [Fact]
    public void UpdateVisitFields_VisitExpired()
    {
        var currentVisitId = Ulid.NewUlid();
        var cookie = new Cookie()
        {
            VisitId = currentVisitId,
            VisitExpiration = Instant.FromUnixTimeMilliseconds(1000)
        };

        var now = Instant.FromUnixTimeMilliseconds(2000);
        var newVisitExpiration = Instant.FromUnixTimeMilliseconds(5000);

        Assert.True(now >= cookie.VisitExpiration);

        var updatedCookie = cookie.UpdateVisitFields(now, newVisitExpiration);

        Assert.NotEqual(currentVisitId, updatedCookie.VisitId);
        Assert.Equal(newVisitExpiration, updatedCookie.VisitExpiration);
    }
}