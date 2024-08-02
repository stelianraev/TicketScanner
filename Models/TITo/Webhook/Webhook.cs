using System.Text.Json.Serialization;

namespace CheckIN.Models.TITo.Webhook
{
    public class Webhook
    {
        [JsonPropertyName("_type")]
        public string Type { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("included_triggers")]
        public List<string> IncludedTriggers { get; set; }

        [JsonPropertyName("custom_data")]
        public DateTime? CustomData { get; set; }

        [JsonPropertyName("deprecated")]
        public bool Deprecated { get; set; }
    }
}
