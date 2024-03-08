namespace Momentum.Analytics.DynamoDb
{
    public class IdentifiedVisit 
    {
        /// <summary>
        /// Gets or sets the pii value.
        /// </summary>
        public string PiiValue { get; set; }

        /// <summary>
        /// Gets or sets the pii type id.
        /// </summary>
        public int PiiTypeId { get; set; }

        /// <summary>
        /// Gets or sets the start of the visit.
        /// </summary>
        public DateTime UtcVisitStart { get; set; }        

        /// <summary>
        /// Gets or sets the conversion funnel step.
        /// </summary>
        public int FunnelStepId { get; set; }

        /// <summary>
        /// Gets or sets the referrer domain
        /// </summary>
        public string? Referrer { get; set; }

        /// <summary>
        /// Gets or sets the utm_source value of the visit.
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// Gets or sets the utm_medium value of the visit.
        /// </summary>
        public string? Medium { get; set; }

        /// <summary>
        /// Gets or sets the timestamp of the identification of the visit.
        /// </summary>
        public DateTime UtcTimestamp { get; set; }
    } // end class
} // end namespace