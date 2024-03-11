using Momentum.Analytics.Core.Visits.Interfaces;

namespace Momentum.Analytics.Core.Visits.Models
{
    public class VisitConfiguration : IVisitConfiguration
    {
        public bool IsSliding { get; set; }

        public TimeSpan WindowLength { get; set; }

        public TimeSpan LocalCacheExpiration { get; set; } 

        public VisitConfiguration()
        {
            IsSliding = false;
            WindowLength = TimeSpan.FromHours(24);
            LocalCacheExpiration = TimeSpan.FromSeconds(10);
        } // end method
    } // end class
} // end namespace