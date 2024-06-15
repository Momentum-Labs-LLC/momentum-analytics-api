using System.Text.Json.Serialization;
using Amazon.Lambda.DynamoDBEvents;
using Amazon.Lambda.SQSEvents;

namespace Momentum.Analytics.Pii.Lambda
{
    [JsonSerializable(typeof(SQSEvent))]
    [JsonSerializable(typeof(DynamoDBEvent))]
    public partial class CustomSerializer : JsonSerializerContext
    {
        
    } // end class
} // end namespace