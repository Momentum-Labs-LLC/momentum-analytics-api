using Microsoft.Extensions.Logging;
using Momentum.Analytics.Core.Interfaces;
using Momentum.Analytics.Core.Visits.Interfaces;
using NodaTime;

namespace Momentum.Analytics.Core.Visits
{
    public class VisitWindowCalculator : IVisitWindowCalculator
    {
        protected readonly IVisitConfiguration _visitConfiguration;
        protected readonly ILogger _logger;

        public VisitWindowCalculator(IVisitConfiguration visitConfiguration, ILogger<VisitWindowCalculator> logger)
        {
            _visitConfiguration = visitConfiguration ?? throw new ArgumentNullException(nameof(visitConfiguration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        } // end method

        public virtual async Task<Instant> GetExpirationAsync(Instant activityTimestamp, CancellationToken token = default)
        {
            Instant result;
            if(_visitConfiguration.IsSliding)
            {
                // visits are configured to be sliding windows
                result = activityTimestamp.Plus(_visitConfiguration.WindowLength);
            }
            else
            {
                // visits are configured to be fixed windows
                var zonedActivityTimestamp = activityTimestamp.InZone(_visitConfiguration.TimeZone);

                var localDate = new LocalDate(
                        zonedActivityTimestamp.Year, 
                        zonedActivityTimestamp.Month, 
                        zonedActivityTimestamp.Day);

                var zonedMidnight = zonedActivityTimestamp.Zone.AtStartOfDay(localDate);
                var firstWindowStart = Duration.FromMinutes(_visitConfiguration.FixedWindowStart.Value.Hour * 60 + _visitConfiguration.FixedWindowStart.Value.Minute);
                var zonedFirstWindow = zonedMidnight.Plus(firstWindowStart);

                result = zonedFirstWindow.ToInstant();
                while(result <= activityTimestamp)
                {
                    result = result.Plus(_visitConfiguration.WindowLength);
                } // end while
            } // end if

            return result;
        } // end method
    } // end class
} // end namespace