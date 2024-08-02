using System.Text.Json.Serialization;

namespace CheckIN.Models.TITo.Event
{
    public class Display
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }
}
