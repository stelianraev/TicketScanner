using System.Text.Json.Serialization;

namespace CheckIN.Models.TITo.Event
{
    public class Section
    {
        [JsonPropertyName("states")]
        public State States { get; set; }
    }
}
