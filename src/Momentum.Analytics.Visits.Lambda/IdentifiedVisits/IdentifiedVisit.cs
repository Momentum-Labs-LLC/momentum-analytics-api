namespace Momentum.Analytics.Visits.Lambda.IdentifiedVisits
{
    public class IdentifiedVisit
    {
        public string Pii { get; set; }
        public int PiiTypeId { get; set; }
        public string VisitStart { get; set; }
        public int FunnelStep { get; set; }
        public string Referrer { get; set; }
        public string Source { get; set; }
        public string Medium { get; set; }
        public string CookieId { get; set; }
    } // end class
} // end namespace