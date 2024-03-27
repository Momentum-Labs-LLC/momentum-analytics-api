using Microsoft.Extensions.Configuration;
using Momentum.Analytics.DynamoDb.Abstractions;
using Momentum.Analytics.DynamoDb.Pii.Interfaces;

namespace Momentum.Analytics.DynamoDb.Pii
{
    public class CollectedPiiTableConfiguration : TableConfigurationBase, ICollectedPiiTableConfiguration
    {
        public string CookieTimestampIndexName { get; protected set; }

        public CollectedPiiTableConfiguration(IConfiguration configuration)
        {
            TableName = configuration.GetValue<string>(CollectedPiiConstants.TABLE_NAME, CollectedPiiConstants.TABLE_NAME_DEFAULT);
            CookieTimestampIndexName = configuration.GetValue<string>(CollectedPiiConstants.COOKIE_TIMESTAMP_INDEX, CollectedPiiConstants.COOKIE_TIMESTAMP_INDEX_DEFAULT);
        } // end method
    } // end class
} // end namespace