using System.Text.Json.Serialization;

namespace Momentum.Analytics.Lambda.Api.PageViews.V2
{
    public class PageViewV2ViewModel
    {
        [JsonPropertyName("referer")]
        public string? Referer { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; } = string.Empty;
    } // end class
} // end namespace