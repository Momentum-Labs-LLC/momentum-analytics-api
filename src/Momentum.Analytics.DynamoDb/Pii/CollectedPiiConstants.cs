using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Momentum.Analytics.DynamoDb.Pii
{
    public static class CollectedPiiConstants
    {
        public const string TABLE_NAME = "COLLECTED_PII_TABLE";
        public const string TABLE_NAME_DEFAULT = "momentum-prd-collected-pii";

        public const string PII_ID = "PiiId";
        public const string COOKIE_ID = "CookieId";
        public const string PII_TYPE_ID = "PiiTypeId";
        public const string UTC_TIMESTAMP = "UtcTimestamp";

        public const string COOKIE_TIMESTAMP_INDEX = "COOKIE_TIMESTAMP_INDEX";
        public const string COOKIE_TIMESTAMP_INDEX_DEFAULT = "CookieTimestampIndex";
    } // end class
} // end namespace