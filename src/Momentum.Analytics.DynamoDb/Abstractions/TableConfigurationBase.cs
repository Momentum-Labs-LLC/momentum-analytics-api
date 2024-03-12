namespace Momentum.Analytics.DynamoDb.Abstractions
{
    public abstract class TableConfigurationBase : ITableConfiguration
    {
        public string TableName { get; protected set; }
    } // end class
} // end namespace