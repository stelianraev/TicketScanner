using CheckIN.Models.TITo.Event;
using Newtonsoft.Json;

namespace CheckIN.Models.TITo
{
    public class TitoEventResponse
    {
        [JsonProperty("events")]
        public EventResponse[] Events { get; set; }

        [JsonProperty("meta")]
        public Meta Meta { get; set; }
    }
}
