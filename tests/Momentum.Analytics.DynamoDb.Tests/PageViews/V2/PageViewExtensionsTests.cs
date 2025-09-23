using Momentum.Analytics.Core.PageViews.V2;
using Momentum.Analytics.DynamoDb.PageViews.V2;
using NodaTime;

using PageViewConstants = Momentum.Analytics.DynamoDb.PageViews.PageViewConstants;

namespace Momentum.Analytics.DynamoDb.Tests.PageViews.V2;

public class PageViewExtensionsTests
{
    [Fact]
    public void PageView_ToDynamoDb()
    {
        var pageView = new PageView()
        {
            CookieId = Guid.NewGuid(),
            VisitId = Ulid.NewUlid(),
            UtcTimestamp = Instant.FromUnixTimeMilliseconds(1000),
            Url = "https://www.example.com",
            Referer = "https://www.example.com"
        };

        var dynamoDb = pageView.ToDynamoDb();

        Assert.Equal(pageView.CookieId.ToString(), dynamoDb[PageViewConstants.COOKIE_ID].S);
        Assert.Equal(pageView.VisitId.ToString(), dynamoDb[PageViewConstants.VISIT_ID].S);
        Assert.Equal(pageView.UtcTimestamp.ToUnixTimeMilliseconds(), long.Parse(dynamoDb[PageViewConstants.UTC_TIMESTAMP].N));
        Assert.Equal(pageView.Url, dynamoDb[PageViewConstants.URL].S);
        Assert.Equal(pageView.Referer, dynamoDb[PageViewConstants.REFERER].S);
    }
}