using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.PII.Models;

namespace Momentum.Analytics.Core.Visits.Interfaces
{
    public interface IVisitSearchRequest : ISearchRequest
    {
        Guid? CookieId { get; }
        DateTime? UtcActivityTimestamp { get; }
        bool? IsIdentified { get; }
        string? PiiValue { get; }
        PiiTypeEnum? PiiType { get; }
        ITimeRange? IdentifiedTimeRange { get; }
    } // end interface
} // end namespace