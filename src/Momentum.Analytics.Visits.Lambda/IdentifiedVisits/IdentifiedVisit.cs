namespace Momentum.Analytics.Visits.Lambda.IdentifiedVisits
{
    public class IdentifiedVisit
    {
        public string Pii { get; set; } = string.Empty;
        public int PiiTypeId { get; set; } = 0;
        public string VisitStart { get; set; } = string.Empty;
        public int FunnelStep { get; set; } = 0;
        public string Referrer { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Medium { get; set; } = string.Empty;
        public string CookieId { get; set; } = string.Empty;
    } // end class
} // end namespace