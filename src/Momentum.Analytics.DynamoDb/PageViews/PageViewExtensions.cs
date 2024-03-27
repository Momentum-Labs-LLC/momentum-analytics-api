using Amazon.DynamoDBv2.Model;
using Momentum.Analytics.Core.PageViews.Models;

namespace Momentum.Analytics.DynamoDb.PageViews
{
    public static class PageViewExtensions
    {
        public static Dictionary<string, AttributeValue> ToDynamoDb(this PageView pageView)
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

        public static PageView ToPageView(this Dictionary<string, AttributeValue> fields)
        {
            var result = new PageView();

            var cookieValue = fields.ReadGuid(PageViewConstants.COOKIE_ID);
            if(cookieValue.HasValue)
            {
                result.CookieId = cookieValue.Value;
            }
            else
            {
                throw new ArgumentNullException(PageViewConstants.COOKIE_ID);
            } // end if

            var timestampValue = fields.ReadDateTime(PageViewConstants.UTC_TIMESTAMP);
            if(timestampValue.HasValue)
            {
                result.UtcTimestamp = timestampValue.Value;
            }
            else
            {
                throw new ArgumentNullException(PageViewConstants.UTC_TIMESTAMP);
            } // end if

            result.Domain = fields.ReadString(PageViewConstants.DOMAIN);
            result.Path = fields.ReadString(PageViewConstants.PATH);
            result.FunnelStep = fields.ReadInteger(PageViewConstants.FUNNEL_STEP);
            result.Referer = fields.ReadString(PageViewConstants.REFERER);
            
            var utmParameters = new List<UrchinTrackingParameter>();            
            var sourceValue = fields.ReadString(PageViewConstants.SOURCE);
            if(!string.IsNullOrWhiteSpace(sourceValue))
            {
                utmParameters.Add(new UrchinTrackingParameter()
                {
                    Parameter = UrchinParameterEnum.Source,
                    Value = sourceValue
                });
            } // end if

            var mediumValue = fields.ReadString(PageViewConstants.MEDIUM);
            if(!string.IsNullOrWhiteSpace(mediumValue))
            {
                utmParameters.Add(new UrchinTrackingParameter()
                {
                    Parameter = UrchinParameterEnum.Source,
                    Value = sourceValue
                });
            } // end if

            if(utmParameters.Any())
            {
                result.UtmParameters = utmParameters;
            } // end if            
            
            return result;
        } // end method
    } // end class
} // end namespace