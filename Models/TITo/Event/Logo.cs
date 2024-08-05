using System.Text.Json.Serialization;

namespace CheckIN.Models.TITo.Event
{
    public class Logo
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("thumb")]
        public Thumb Thumb { get; set; }

        [JsonPropertyName("display")]
        public Display Display { get; set; }
    }
}
