using CsvHelper.Configuration;
using Momentum.Analytics.Visits.Lambda.IdentifiedVisits.Interfaces;

namespace Momentum.Analytics.Visits.Lambda.IdentifiedVisits
{
    public interface IIdentifiedVisitMap { } // end simple interface
    public class IdentifiedVisitMap : ClassMap<IdentifiedVisit>, IIdentifiedVisitMap
    {
        public IdentifiedVisitMap(IColumnNameConfiguration columnNameConfiguration)
        {
            Map(m => m.Pii).Name(columnNameConfiguration.PiiColumn);
            Map(m => m.PiiTypeId).Name(columnNameConfiguration.PiiTypeIdColumn);
            Map(m => m.VisitStart).Name(columnNameConfiguration.VisitStartColumn);
            Map(m => m.FunnelStep).Name(columnNameConfiguration.FunnelStepColumn);
            Map(m => m.Referrer).Name(columnNameConfiguration.ReferrerColumn);
            Map(m => m.Source).Name(columnNameConfiguration.SourceColumn);
            Map(m => m.Medium).Name(columnNameConfiguration.MediumColumn);

            if(columnNameConfiguration.WriteCookieId)
            {
                Map(m => m.CookieId).Name(columnNameConfiguration.CookieIdColumn);
            } // end if
        } // end method
    } // end class
} // end namespace