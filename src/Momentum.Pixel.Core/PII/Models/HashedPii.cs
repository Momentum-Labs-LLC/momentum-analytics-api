namespace Momentum.Pixel.Core.PII.Models
{
    /// <summary>
    /// A way to capture pii without directly associating it with the user it came from.
    /// </summary>
    public class HashedPii
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the hashed pii value.
        /// </summary>
        public string Hash { get; set; }

        /// <summary>
        /// Gets or sets the algorith used to hash the PII.
        /// </summary>
        public HashAlgorithmEnum HashAlgorithm { get; set; }     
    } // end class
} // end namespace