using System.Text.Json.Serialization;

namespace Momentum.Analytics.Lambda.Api.PageViews.ViewModels
{
    public class PageViewViewModel
    {
        [JsonPropertyName("referer")]
        public string? Referer { get; set; }

        [JsonPropertyName("utmParameters")]
        public Dictionary<string, string>? UtmParameters { get; set; }

        [JsonPropertyName("domain")]
        public string Domain { get; set; } = string.Empty;

        [JsonPropertyName("path")]
        public string? Path { get; set; } = "/";

        [JsonPropertyName("funnelStep")]
        public int FunnelStep { get; set; } = 0;
    } // end class
} // end namespace