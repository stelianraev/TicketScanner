using Newtonsoft.Json;

namespace CheckIN.Models.TITo.Webhook
{
    public class WebhookEndpoint
    {
        [JsonProperty("_type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("included_triggers")]
        public List<string> IncludedTriggers { get; set; }

        [JsonProperty("custom_data")]
        public DateTime? CustomData { get; set; }

        [JsonProperty("deprecated")]
        public bool Deprecated { get; set; }
    }
}
