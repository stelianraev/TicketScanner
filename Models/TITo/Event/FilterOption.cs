using Newtonsoft.Json;

namespace CheckIN.Models.TITo.Event
{
    public class FilterOption
    {
        [JsonProperty("sections")]
        public Section Section { get; set; }

        [JsonProperty("collection")]
        public bool Collection { get; set; }

        [JsonProperty("states")]
        public List<State> States { get; set; }

        [JsonProperty("selected_states")]
        public List<string> SelectedStates { get; set; }
    }
}
