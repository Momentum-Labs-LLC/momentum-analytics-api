using Momentum.Analytics.DynamoDb.Abstractions;

namespace Momentum.Analytics.DynamoDb.Visits.Interfaces
{
    public interface IVisitTableConfiguration : ITableConfiguration
    {
        string VisitStartIndex { get; }
        string IdentifiedIndex { get; }
        //string UnidentifiedIndex { get; }
    } // end interface
} // end namespace