using CheckIN.Models.TITo.Event;
using Newtonsoft.Json;

namespace CheckIN.Models.TITo.Webhook
{
    public class WebhookResponse
    {
        [JsonProperty("webhook_endpoints")]
        public List<WebhookEndpoint> WebhookEndpoints { get; set; }


        [JsonProperty("meta")]
        public MetaData Meta { get; set; }
    }
}
