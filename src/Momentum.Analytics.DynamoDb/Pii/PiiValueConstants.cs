using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Momentum.Analytics.DynamoDb.Pii
{
    public static class PiiValueConstants
    {
        public const string TABLE_NAME = "PII_VALUE_TABLE";
        public const string TABLE_NAME_DEFAULT = "momentum-prd-pii-values";
        public const string ID_INDEX_NAME = "PII_ID_INDEX_NAME";
        public const string ID_INDEX_NAME_DEFAULT = "IdIndex";

        public const string PII_ID = "PiiId";
        public const string VALUE = "Value";
        public const string PII_TYPE = "PiiType";
        public const string HASH_ALGORITHM = "HashAlgorithm";        
        public const string UTC_TIMESTAMP = "UtcTimestamp";
    } // end class
} // end namespace