using Amazon.DynamoDBv2.Model;
using Momentum.Analytics.Core.PageViews.V2;

namespace Momentum.Analytics.DynamoDb.PageViews.V2
{
    public static class PageViewExtensions
    {
        public static Dictionary<string, AttributeValue> ToDynamoDb(this PageView pageView)
        {
            return new Dictionary<string, AttributeValue>()
                .AddField(PageViewConstants.COOKIE_ID, pageView.CookieId)
                .AddField(PageViewConstants.VISIT_ID, pageView.VisitId.ToString())
                .AddField(PageViewConstants.UTC_TIMESTAMP, pageView.UtcTimestamp)
                .AddField(PageViewConstants.URL, pageView.Url)
                .AddField(PageViewConstants.REFERER, pageView.Referer);
        }

        public static PageView ToDomain(this Dictionary<string, AttributeValue> fields)
        {
            return new PageView()
            {
                CookieId = fields.ReadGuid(PageViewConstants.COOKIE_ID, true).Value,
                VisitId = fields.ReadUlid(PageViewConstants.VISIT_ID),
                UtcTimestamp = fields.ReadDateTime(PageViewConstants.UTC_TIMESTAMP, true).Value,
                Url = fields.ReadString(PageViewConstants.URL),
                Referer = fields.ReadString(PageViewConstants.REFERER)
            };
        }
    }
}