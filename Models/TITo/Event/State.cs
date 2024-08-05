using System.Text.Json.Serialization;

namespace CheckIN.Models.TITo.Event
{
    public class State
    {
        [JsonPropertyName("label")]
        public string Label { get; set; }

        [JsonPropertyName("open")]
        public bool Open { get; set; }
    }
}
