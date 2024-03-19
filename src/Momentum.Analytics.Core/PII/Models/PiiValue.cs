namespace Momentum.Analytics.Core.PII.Models
{
    /// <summary>
    /// A way to capture pii without directly associating it with the user it came from.
    /// </summary>
    public class PiiValue
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the hashed pii value.
        /// </summary>
        public string Value { get; set; }
        
        /// <summary>
        /// Gets or sets the type of pii.
        /// </summary>
        public PiiTypeEnum PiiType { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        public DateTime UtcTimestamp { get; set; }
    } // end class
} // end namespace