using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using Momentum.Analytics.Core.PII.Models;

namespace Momentum.Analytics.DynamoDb.Pii
{
    public static class CollectedPiiExtensions
    {
        public static Dictionary<string, AttributeValue> ToDynamoDb(this CollectedPii collectedPii)
        {
            return new Dictionary<string, AttributeValue>()
                .AddField(CollectedPiiConstants.PII_ID, collectedPii.PiiId.ToString())
                .AddField(CollectedPiiConstants.COOKIE_ID, collectedPii.CookieId)
                .AddField(CollectedPiiConstants.UTC_TIMESTAMP, collectedPii.UtcTimestamp);
        } // end method

        public static CollectedPii ReadCollectedPii(this Dictionary<string, AttributeValue> fields)
        {
            var result = new CollectedPii();

            result.PiiId = fields.ReadGuid(CollectedPiiConstants.PII_ID, true);
            result.CookieId = fields.ReadGuid(CollectedPiiConstants.COOKIE_ID, true).Value;
            result.UtcTimestamp = fields.ReadDateTime(CollectedPiiConstants.UTC_TIMESTAMP, true).Value;

            return result;
        } // end method
    } // end class
} // end namespace