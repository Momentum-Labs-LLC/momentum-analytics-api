namespace Momentum.Analytics.DynamoDb
{
    public class CollectedPii
    {
        public Guid PiiId { get; set; }
        public Guid CookieId { get; set; }
        public DateTime UtcTimestamp { get; set; }
    } // end class
} // end namespace