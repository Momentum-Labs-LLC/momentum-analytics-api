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
                .AddField(CollectedPiiConstants.VISIT_ID, collectedPii.VisitId.ToString())
                .AddField(CollectedPiiConstants.UTC_TIMESTAMP, collectedPii.UtcTimestamp)
                .AddField(CollectedPiiConstants.PII_TYPE_ID, (int)collectedPii.Pii?.PiiType);
        } // end method

        public static CollectedPii ReadCollectedPii(this Dictionary<string, AttributeValue> fields)
        {
            var result = new CollectedPii();

            result.PiiId = fields.ReadGuid(CollectedPiiConstants.PII_ID, true);
            result.VisitId = fields.ReadUlid(CollectedPiiConstants.VISIT_ID);
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
    } // end class
} // end namespace