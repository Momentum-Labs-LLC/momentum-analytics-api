using Momentum.Analytics.Core.Models;

namespace Momentum.Analytics.Core.PII.Models
{
    /// <summary>
    /// A class representing where a given piece of PII is from.
    /// </summary>
    public class CollectedPii : UserActivity
    {
        public Guid PiiId { get; set; }
        public PiiValue? Pii { get; set; }
    } // end class
} // end namespace