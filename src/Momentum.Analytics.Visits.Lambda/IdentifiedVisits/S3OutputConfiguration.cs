using Microsoft.Extensions.Configuration;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Visits.Interfaces;
using Momentum.Analytics.Visits.Lambda.IdentifiedVisits.Interfaces;

namespace Momentum.Analytics.Visits.Lambda.IdentifiedVisits
{
    public class S3OutputConfiguration : IS3OutputConfiguration
    {
        public const string OUTPUT_BUCKET = "OUTPUT_BUCKET";
        public const string OUTPUT_BUCKET_DEFAULT = "momentum-prd-visits";

        public const string FUNNEL_STEP_COLUMN = "FUNNEL_STEP_COLUMN";
        public const string FUNNEL_STEP_COLUMN_DEFAULT = "FunnelStep";

        public string Bucket { get; protected set; }
        protected readonly IVisitConfiguration _visitConfiguration;

        public S3OutputConfiguration(IConfiguration configuration, IVisitConfiguration visitConfiguration)
        {
            Bucket = configuration.GetValue<string>(OUTPUT_BUCKET, OUTPUT_BUCKET_DEFAULT);
            _visitConfiguration = visitConfiguration ?? throw new ArgumentNullException(nameof(visitConfiguration));
        } // end method

        public virtual Task<string> BuildKeyAsync(ITimeRange timeRange, CancellationToken token = default)
        {
            var startZoned = timeRange.UtcStart.InUtc();
            var endZoned = timeRange.UtcEnd.InUtc();

            if(_visitConfiguration.TimeZone != null)
            {
                startZoned = timeRange.UtcStart.InZone(_visitConfiguration.TimeZone);
                endZoned = timeRange.UtcEnd.InZone(_visitConfiguration.TimeZone);
            } // end if
            
            return Task.FromResult($"{startZoned.ToString("yyyyMM", null)}/{startZoned.ToString("yyyyMMddHH", null)}_{endZoned.ToString("yyyyMMddHH", null)}.csv");
        } // end method
    } // end class
} // end namespace