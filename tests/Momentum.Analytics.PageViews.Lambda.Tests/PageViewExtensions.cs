using Momentum.Analytics.Core.PageViews.Models;
using static Amazon.Lambda.DynamoDBEvents.DynamoDBEvent;
using Momentum.Analytics.Processing.DynamoDb;
using PageViewConstants = Momentum.Analytics.DynamoDb.PageViews.PageViewConstants;

namespace Momentum.Analytics.PageViews.Lambda.Tests;

public static class PageViewExtensions
{
    public static Dictionary<string, AttributeValue> ToDynamoDbEvent(this PageView pageView)
    {
        var result = new Dictionary<string, AttributeValue>()
            .AddField(PageViewConstants.COOKIE_ID, pageView.CookieId)
            .AddField(PageViewConstants.UTC_TIMESTAMP, pageView.UtcTimestamp)
            .AddField(PageViewConstants.DOMAIN, pageView.Domain)
            .AddField(PageViewConstants.PATH, pageView.Path)
            .AddField(PageViewConstants.FUNNEL_STEP, pageView.FunnelStep)
            .AddField(PageViewConstants.REFERER, pageView.Referer);

        var sourceValue = pageView.UtmParameters?.FirstOrDefault(x => x.Parameter == UrchinParameterEnum.Source)?.Value;
        if(!string.IsNullOrWhiteSpace(sourceValue))
        {
            result.AddField(PageViewConstants.SOURCE, sourceValue);
        } // end if

        var mediumValue = pageView.UtmParameters?.FirstOrDefault(x => x.Parameter == UrchinParameterEnum.Medium)?.Value;
        if(!string.IsNullOrWhiteSpace(mediumValue))
        {
            result.AddField(PageViewConstants.MEDIUM, mediumValue);
        } // end if                

        return result;
    } // end method
} // end class