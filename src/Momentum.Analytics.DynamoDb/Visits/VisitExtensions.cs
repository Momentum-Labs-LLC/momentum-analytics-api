using Amazon.DynamoDBv2.Model;
using Momentum.Analytics.Core.Extensions;
using Momentum.Analytics.Core.PII.Models;
using Momentum.Analytics.Core.Visits.Models;
using NodaTime;
using NodaTime.Extensions;

namespace Momentum.Analytics.DynamoDb.Visits
{
    public static class VisitExtensions
    {
        public static Instant EpochInstant = DateTime.UnixEpoch.ToInstant().TrimToHour();
        public static Dictionary<string, AttributeValue> ToDynamoDb(this Visit visit)
        {
            return new Dictionary<string, AttributeValue>()
                .AddField(VisitConstants.ID, visit.Id)
                .AddField(VisitConstants.COOKIE_ID, visit.CookieId)
                .AddField(VisitConstants.UTC_START, visit.UtcStart)
                .AddField(VisitConstants.UTC_START_HOUR, visit.UtcStart.TrimToHour())
                .AddField(VisitConstants.UTC_EXPIRATION, visit.UtcExpiration)
                .AddField(VisitConstants.FUNNEL_STEP, visit.FunnelStep)
                .AddField(VisitConstants.REFERER, visit.Referer)
                .AddField(VisitConstants.SOURCE, visit.Source)
                .AddField(VisitConstants.MEDIUM, visit.Medium)
                .AddField(VisitConstants.PII_VALUE, visit.PiiValue)
                .AddField(VisitConstants.PII_TYPE, (int?)visit.PiiType)
                .AddField(VisitConstants.IS_IDENTIFIED, visit.UtcIdentifiedTimestamp.HasValue)
                .AddField(VisitConstants.UTC_IDENTIFIED_TIMESTAMP, visit.UtcIdentifiedTimestamp)
                .AddField(VisitConstants.UTC_IDENTIFIED_HOUR, visit.UtcIdentifiedTimestamp.HasValue ? 
                                visit.UtcIdentifiedTimestamp.Value.TrimToHour() : EpochInstant);
        } // end method

        public static Visit ToVisit(this Dictionary<string, AttributeValue> fields)
        {
            var result = new Visit();
            result.Id = fields.ReadGuid(VisitConstants.ID, true).Value;
            result.CookieId = fields.ReadGuid(VisitConstants.COOKIE_ID, true).Value;
            result.UtcStart = fields.ReadDateTime(VisitConstants.UTC_START, true).Value;
            result.UtcExpiration = fields.ReadDateTime(VisitConstants.UTC_EXPIRATION);
            result.FunnelStep = fields.ReadInteger(VisitConstants.FUNNEL_STEP);
            result.Referer = fields.ReadString(VisitConstants.REFERER);
            result.Source = fields.ReadString(VisitConstants.SOURCE);
            result.Medium = fields.ReadString(VisitConstants.MEDIUM);
            result.PiiValue = fields.ReadString(VisitConstants.PII_VALUE);
            result.PiiType = (PiiTypeEnum?)fields.ReadNullableInteger(VisitConstants.PII_TYPE);
            result.UtcIdentifiedTimestamp = fields.ReadDateTime(VisitConstants.UTC_IDENTIFIED_TIMESTAMP);

            return result;
        } // end method
    } // end class
} // end namespace