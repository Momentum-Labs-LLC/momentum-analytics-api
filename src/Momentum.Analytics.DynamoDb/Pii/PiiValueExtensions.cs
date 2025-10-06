using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.Model;
using Microsoft.VisualBasic;
using Momentum.Analytics.Core.PII.Models;

namespace Momentum.Analytics.DynamoDb.Pii
{
    public static class PiiValueExtensions
    {
        public static Dictionary<string, AttributeValue> ToDynamoDb(this PiiValue piiValue)
        {
            return new Dictionary<string, AttributeValue>()
                .AddField(PiiValueConstants.PII_ID, piiValue.Id)
                .AddField(PiiValueConstants.VALUE, piiValue.Value)
                .AddField(PiiValueConstants.PII_TYPE, (int)piiValue.PiiType)
                .AddField(PiiValueConstants.UTC_TIMESTAMP, piiValue.UtcTimestamp);
        } // end method

        public static PiiValue ReadPiiValue(this Dictionary<string, AttributeValue> fields)
        {
            PiiValue result = new PiiValue();
            var piiId = fields.ReadGuid(PiiValueConstants.PII_ID);
            if(piiId.HasValue)
            {
                result.Id = piiId.Value;
            }
            else
            {
                throw new ArgumentNullException(PiiValueConstants.PII_ID);
            } // end if

            result.Value = fields.ReadString(PiiValueConstants.VALUE) ?? string.Empty;
            result.PiiType = (PiiTypeEnum)fields.ReadInteger(PiiValueConstants.PII_TYPE);
            var timestamp = fields.ReadDateTime(PiiValueConstants.UTC_TIMESTAMP);
            if(timestamp.HasValue)
            {
                result.UtcTimestamp = timestamp.Value;
            }
            else
            {
                throw new ArgumentNullException(PiiValueConstants.UTC_TIMESTAMP);
            } // end if

            return result;
        } // end method
    } // end class
} // end namespace