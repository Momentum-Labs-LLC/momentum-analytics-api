using Microsoft.Extensions.Configuration;
using Momentum.Analytics.DynamoDb.Abstractions;
using Momentum.Analytics.DynamoDb.Pii.Interfaces;

namespace Momentum.Analytics.DynamoDb.Pii
{
    public class PiiValueTableConfiguration : TableConfigurationBase, IPiiValueTableConfiguration
    {
        public PiiValueTableConfiguration(IConfiguration configuration)
        {
            TableName = configuration.GetValue<string>(PiiValueConstants.TABLE_NAME, PiiValueConstants.TABLE_NAME_DEFAULT);
        } // end method
    } // end class
} // end namespace