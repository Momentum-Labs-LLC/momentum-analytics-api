using Momentum.Analytics.DynamoDb.Abstractions;

namespace Momentum.Analytics.DynamoDb.Pii.Interfaces
{
    public interface ICollectedPiiTableConfiguration : ITableConfiguration
    {
        string CookieTimestampIndexName { get; }
    } // end interface
} // end namespace