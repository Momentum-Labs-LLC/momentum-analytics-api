namespace Momentum.Analytics.Core.PII.Models
{
    /// <summary>
    /// A class representing where a given piece of PII is from.
    /// </summary>
    public class CollectedPii
    {
        public Guid Id { get; set; }
        public Guid CookieId { get; set; }
        public DateTime UtcTimestamp { get; set; }
        public PiiValue Pii { get; set; }
    } // end class
} // end namespace