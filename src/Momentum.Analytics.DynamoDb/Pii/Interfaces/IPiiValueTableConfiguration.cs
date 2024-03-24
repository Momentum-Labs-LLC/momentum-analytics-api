using Momentum.Analytics.DynamoDb.Abstractions;

namespace Momentum.Analytics.DynamoDb.Pii.Interfaces
{
    public interface IPiiValueTableConfiguration : ITableConfiguration
    {
        string IdIndexName { get; }
    } // end interface
} // end namespace