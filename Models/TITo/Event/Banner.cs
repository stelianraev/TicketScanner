using System.Text.Json.Serialization;

namespace CheckIN.Models.TITo.Event
{
    public class Banner
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("thumb")]
        public Thumb Thumb { get; set; }
    }
}
