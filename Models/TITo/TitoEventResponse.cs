using CheckIN.Models.TITo.Event;
using System.Text.Json.Serialization;

namespace CheckIN.Models.TITo
{
    public class TitoEventResponse
    {
        [JsonPropertyName("events")]
        public EventResponse[] Events { get; set; }

        [JsonPropertyName("meta")]
        public Meta Meta { get; set; }
    }
}
