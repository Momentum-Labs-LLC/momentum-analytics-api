namespace Momentum.Analytics.Visits.Lambda
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
    } // end interface
} // end namespace