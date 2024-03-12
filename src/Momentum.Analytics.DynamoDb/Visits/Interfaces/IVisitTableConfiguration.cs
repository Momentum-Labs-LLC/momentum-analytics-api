using Momentum.Analytics.DynamoDb.Abstractions;

namespace Momentum.Analytics.DynamoDb.Visits.Interfaces
{
    public interface IVisitTableConfiguration : ITableConfiguration
    {
        string VisitExpirationIndex { get; }
        string IdentifiedIndex { get; }
    } // end interface
} // end namespace