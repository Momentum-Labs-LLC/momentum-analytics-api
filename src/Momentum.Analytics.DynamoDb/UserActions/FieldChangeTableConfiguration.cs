using Microsoft.Extensions.Configuration;
using Momentum.Analytics.DynamoDb.Abstractions;

namespace Momentum.Analytics.DynamoDb.UserActions;

public interface IFieldChangeTableConfiguration : ITableConfiguration
{
} // end interface


public class FieldChangeTableConfiguration : TableConfigurationBase, IFieldChangeTableConfiguration
{
    public FieldChangeTableConfiguration(IConfiguration configuration)
    {
        TableName = configuration.GetValue<string>(FieldChangeConstants.TABLE_NAME, FieldChangeConstants.TABLE_NAME_DEFAULT);
    } // end method
} // end class