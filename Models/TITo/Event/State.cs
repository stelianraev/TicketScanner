using Newtonsoft.Json;

namespace CheckIN.Models.TITo.Event
{
    public class State
    {
        [JsonProperty("label")]
        public string Label { get; set; }

        [JsonProperty("open")]
        public bool Open { get; set; }
    }
}
