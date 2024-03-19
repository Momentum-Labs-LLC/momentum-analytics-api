using System.Text.Json.Serialization;
using Momentum.Analytics.Core.PII.Models;

namespace Momentum.Analytics.Lambda.Api.Pii.ViewModels
{
    public class PiiViewModel
    {
        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("type")]
        public PiiTypeEnum Type { get; set; }
    } // end class
} // end namespace