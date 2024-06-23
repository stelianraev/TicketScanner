using Newtonsoft.Json;

namespace CheckIN.Models.TITo.Event
{
    public class Banner
    {
        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("thumb")]
        public Thumb Thumb { get; set; }
    }
}
