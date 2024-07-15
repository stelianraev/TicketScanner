using Newtonsoft.Json;

namespace CheckIN.Models.TITo.Event
{
    public class Logo
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("thumb")]
        public Thumb Thumb { get; set; }

        [JsonProperty("display")]
        public Display Display { get; set; }
    }
}
