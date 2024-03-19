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
        public const string UTC_TIMESTAMP = "UtcTimestamp";
    } // end class
} // end namespace