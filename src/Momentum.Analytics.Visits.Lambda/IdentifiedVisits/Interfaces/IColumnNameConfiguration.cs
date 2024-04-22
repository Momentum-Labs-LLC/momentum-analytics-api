namespace Momentum.Analytics.Visits.Lambda.IdentifiedVisits.Interfaces
{
    public interface IColumnNameConfiguration
    {
        string PiiColumn { get; }
        string PiiTypeIdColumn { get; }
        string VisitStartColumn { get; }
        string FunnelStepColumn { get; }
        string ReferrerColumn { get; }
        string SourceColumn { get; }
        string MediumColumn { get; }
        bool WriteCookieId { get; }
        string CookieIdColumn { get; }
    } // end interface
} // end namespace