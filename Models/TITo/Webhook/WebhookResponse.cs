using CheckIN.Models.TITo.Event;
using System.Text.Json.Serialization;

namespace CheckIN.Models.TITo.Webhook
{
    public class WebhookResponse
    {
        [JsonPropertyName("webhook_endpoints")]
        public List<Webhook> WebhookEndpints { get; set; }

        [JsonPropertyName("meta")]
        public MetaData Meta { get; set; }
    }
}
