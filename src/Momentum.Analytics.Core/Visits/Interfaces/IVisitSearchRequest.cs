using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.PII.Models;

namespace Momentum.Analytics.Core.Visits.Interfaces
{
    public interface IVisitSearchRequest<TPage> : ISearchRequest<TPage>
    {
        Guid? CookieId { get; }
        DateTime? UtcActivityTimestamp { get; }
        bool? IsIdentified { get; }
        string? PiiValue { get; }
        PiiTypeEnum? PiiType { get; }
        ITimeRange? IdentifiedTimeRange { get; }
    } // end interface

    public interface IVisitSearchRequest : IVisitSearchRequest<int> {} // end interface
} // end namespace