using Momentum.Analytics.DynamoDb.Abstractions;

namespace Momentum.Analytics.DynamoDb.Visits.Interfaces
{
    public interface IVisitTableConfiguration : ITableConfiguration
    {
        string CookieIndex { get; }
        string VisitStartIndex { get; }
        string IdentifiedIndex { get; }
    } // end interface
} // end namespace