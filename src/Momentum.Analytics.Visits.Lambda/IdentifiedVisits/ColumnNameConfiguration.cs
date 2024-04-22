using Microsoft.Extensions.Configuration;
using Momentum.Analytics.Visits.Lambda.IdentifiedVisits.Interfaces;

namespace Momentum.Analytics.Visits.Lambda.IdentifiedVisits
{
    public class ColumnNameConfiguration : IColumnNameConfiguration
    {
        public string PiiColumn { get; protected set; }

        public string PiiTypeIdColumn { get; protected set; }

        public string VisitStartColumn { get; protected set; }

        public string FunnelStepColumn { get; protected set; }

        public string ReferrerColumn { get; protected set; }

        public string SourceColumn { get; protected set; }

        public string MediumColumn { get; protected set; }
        public bool WriteCookieId { get; protected set; }
        public string CookieIdColumn { get; protected set; }

        public const string PII_COLUMN = "PII_COLUMN";
        public const string PII_COLUMN_DEFAULT = "Pii";
        public const string PII_TYPE_ID_COLUMN = "PII_TYPE_ID";
        public const string PII_TYPE_ID_DEFAULT = "PiiTypeId";
        public const string VISIT_START_COLUMN = "VISIT_START";
        public const string VISIT_START_COLUMN_DEFAULT = "VisitStart";
        public const string FUNNEL_STEP_COLUMN = "FUNNEL_STEP";
        public const string FUNNEL_STEP_COLUMN_DEFAULT = "FunnelStep";
        public const string REFERRER_COLUMN = "REFERRER_COLUMN";
        public const string REFERRER_COLUMN_DEFAULT = "Referrer";
        public const string SOURCE_COLUMN = "SOURCE";
        public const string SOURCE_COLUMN_DEFAULT = "Source";
        public const string MEDIUM_COLUMN = "MEDIUM";
        public const string MEDIUM_COLUMN_DEFAULT = "Medium";
        public const string WRITE_COOKIE_ID = "WRITE_COOKIE_ID";
        public const bool WRITE_COOKIE_ID_DEFAULT = false;
        public const string COOKIE_ID_COLUMN = "COOKIE_ID";
        public const string COOKIE_ID_COLUMN_DEFAULT = "CookieId";

        public ColumnNameConfiguration(IConfiguration configuration)
        {
            PiiColumn = configuration.GetValue<string>(PII_COLUMN, PII_COLUMN_DEFAULT);
            PiiTypeIdColumn = configuration.GetValue<string>(PII_TYPE_ID_COLUMN, PII_TYPE_ID_DEFAULT);
            VisitStartColumn = configuration.GetValue<string>(VISIT_START_COLUMN, VISIT_START_COLUMN_DEFAULT);
            FunnelStepColumn = configuration.GetValue<string>(FUNNEL_STEP_COLUMN, FUNNEL_STEP_COLUMN_DEFAULT);
            ReferrerColumn = configuration.GetValue<string>(REFERRER_COLUMN, REFERRER_COLUMN_DEFAULT);
            SourceColumn = configuration.GetValue<string>(SOURCE_COLUMN, SOURCE_COLUMN_DEFAULT);
            MediumColumn = configuration.GetValue<string>(MEDIUM_COLUMN, MEDIUM_COLUMN_DEFAULT);
            WriteCookieId = configuration.GetValue<bool>(WRITE_COOKIE_ID, WRITE_COOKIE_ID_DEFAULT);
            CookieIdColumn = configuration.GetValue<string>(COOKIE_ID_COLUMN, COOKIE_ID_COLUMN_DEFAULT);
        } // end method
    } // end class
} // end namespace