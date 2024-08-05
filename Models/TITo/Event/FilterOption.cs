using System.Text.Json.Serialization;

namespace CheckIN.Models.TITo.Event
{
    public class FilterOption
    {
        [JsonPropertyName("sections")]
        public Section Section { get; set; }

        [JsonPropertyName("collection")]
        public bool Collection { get; set; }

        [JsonPropertyName("states")]
        public List<State> States { get; set; }

        [JsonPropertyName("selected_states")]
        public List<string> SelectedStates { get; set; }
    }
}
