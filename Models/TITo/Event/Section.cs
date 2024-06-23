using Newtonsoft.Json;

namespace CheckIN.Models.TITo.Event
{
    public class Section
    {
        [JsonProperty("states")]
        public State States { get; set; }
    }
}
