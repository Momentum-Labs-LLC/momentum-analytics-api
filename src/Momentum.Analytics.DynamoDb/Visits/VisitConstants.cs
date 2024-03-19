using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;

namespace Momentum.Analytics.DynamoDb.Visits
{
    public static class VisitConstants
    {
        public const string TABLE_NAME = "VISIT_TABLE";
        public const string TABLE_NAME_DEFAULT = "visits";

        public const string EXPIRATION_INDEX = "VISIT_EXPIRATION_INDEX";
        public const string EXPIRATION_INDEX_DEFAULT = "VisitExpirationIndex";

        public const string IDENTIFIED_INDEX = "IDENTIFIED_INDEX";
        public const string IDENTIFIED_INDEX_DEFAULT = "IdentifiedIndex";

        public const string ID = "Id";
        public const string COOKIE_ID = "CookieId";
        public const string UTC_START = "UtcStart";
        public const string UTC_EXPIRATION = "UtcExpiration";
        public const string FUNNEL_STEP = "FunnelStep";
        public const string REFERER = "Referer";
        public const string SOURCE = "Source";
        public const string MEDIUM = "Medium";
        public const string PII_VALUE = "PiiValue";
        public const string PII_TYPE = "PiiType";
        public const string IS_IDENTIFIED = "IsIdentified";
        public const string UTC_IDENTIFIED_TIMESTAMP = "UtcIdentifiedTimestamp";
    } // end class
} // end namespace