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
                .AddField(CollectedPiiConstants.COOKIE_ID, collectedPii.CookieId)
                .AddField(CollectedPiiConstants.UTC_TIMESTAMP, collectedPii.UtcTimestamp);
        } // end method

        public static CollectedPii ReadCollectedPii(this Dictionary<string, AttributeValue> fields)
        {
            var result = new CollectedPii();

            var piiId = fields.ReadGuid(CollectedPiiConstants.PII_ID);
            if(piiId.HasValue)
            {
                result.PiiId = piiId.Value;
            }
            else
            {
                throw new ArgumentNullException(CollectedPiiConstants.PII_ID);
            } // end if

            var cookieId = fields.ReadGuid(CollectedPiiConstants.COOKIE_ID);
            if(cookieId.HasValue)
            {
                result.CookieId = cookieId.Value;
            }
            else
            {
                throw new ArgumentNullException(CollectedPiiConstants.COOKIE_ID);
            } // end if

            var timestamp = fields.ReadDateTime(CollectedPiiConstants.UTC_TIMESTAMP);
            if(timestamp.HasValue)
            {
                result.UtcTimestamp = timestamp.Value;
            }
            else
            {
                throw new ArgumentNullException(CollectedPiiConstants.UTC_TIMESTAMP);
            } // end if

            return result;
        } // end method
    } // end class
} // end namespace