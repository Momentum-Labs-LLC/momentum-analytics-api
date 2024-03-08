namespace Momentum.Analytics.DynamoDb
{
    public class PiiValue
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public int HashAlgorithmId { get; set; }
        public int PiiTypeId { get; set; }
        public DateTime UtcTimestamp { get; set; }
    } // end class
} // end namespace