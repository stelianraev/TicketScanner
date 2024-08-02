using System.Text.Json.Serialization;

namespace CheckIN.Models.TITo.Event
{
    public class Meta
    {
        [JsonPropertyName("current_page")]
        public int CurrentPage { get; set; }

        [JsonPropertyName("next_page")]
        public string NextPage { get; set; }

        [JsonPropertyName("prev_page")]
        public string PrevPage { get; set; }

        [JsonPropertyName("total_pages")]
        public int TotalPages { get; set; }

        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }

        [JsonPropertyName("per_page")]
        public int PerPage { get; set; }

        [JsonPropertyName("filter_options")]
        public FilterOption FilterOptions { get; set; }
    }
}
