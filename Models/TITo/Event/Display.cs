using Newtonsoft.Json;

namespace CheckIN.Models.TITo.Event
{
    public class Display
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
