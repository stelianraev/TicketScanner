using System.Text.Json.Serialization;

namespace CheckIN.Models.TITo.Event
{
    public class Thumb
    {
        [JsonPropertyName("url")]
        public string url { get; set; }
    }
}
