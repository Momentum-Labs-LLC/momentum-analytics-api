using Momentum.Analytics.Core.PII.Models;
using static Amazon.Lambda.DynamoDBEvents.DynamoDBEvent;

using CollectedPiiConstants = Momentum.Analytics.DynamoDb.Pii.CollectedPiiConstants;

namespace Momentum.Analytics.Processing.DynamoDb.Pii;

public static class PiiExtensions
{
    public static CollectedPii ReadCollectedPii(this Dictionary<string, AttributeValue> fields)
    {
        var result = new CollectedPii();

        result.PiiId = fields.ReadGuid(CollectedPiiConstants.PII_ID, true);
        result.CookieId = fields.ReadGuid(CollectedPiiConstants.COOKIE_ID, true).Value;
        result.UtcTimestamp = fields.ReadDateTime(CollectedPiiConstants.UTC_TIMESTAMP, true).Value;

        var piiTypeId = fields.ReadNullableInteger(CollectedPiiConstants.PII_TYPE_ID);
        if(piiTypeId.HasValue)
        {
            result.Pii = new PiiValue()
            {
                Id = result.PiiId.Value,
                PiiType = (PiiTypeEnum)piiTypeId.Value
            };
        } // end if

        return result;
    } // end method
}