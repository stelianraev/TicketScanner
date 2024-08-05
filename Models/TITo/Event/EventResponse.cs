using System.Text.Json.Serialization;

namespace CheckIN.Models.TITo.Event
{
    public class EventResponse
    {
        [JsonPropertyName("_type")]
        public string Type { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public string UpdatedAt { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("banner")]
        public Banner Banner { get; set; }

        [JsonPropertyName("banner_url")]
        public string BannerUrl { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("default_locale")]
        public string DefaultLocale { get; set; }

        [JsonPropertyName("live")]
        public bool Live { get; set; }

        [JsonPropertyName("test_mode")]
        public bool TestMode { get; set; }

        [JsonPropertyName("locales")]
        public List<string> Locales { get; set; }

        [JsonPropertyName("location")]
        public string Location { get; set; }

        [JsonPropertyName("logo")]
        public Logo Logo { get; set; }

        [JsonPropertyName("private")]
        public bool Private { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("account_id")]
        public string AccountId { get; set; }

        [JsonPropertyName("start_date")]
        public string StartDate { get; set; }

        [JsonPropertyName("end_date")]
        public string EndDate { get; set; }

        [JsonPropertyName("date_or_range")]
        public string DateOrRange { get; set; }

        [JsonPropertyName("security_token")]
        public string SecurityToken { get; set; }

        [JsonPropertyName("metadata")]
        public MetaData MetaData { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("setup")]
        public bool Setup { get; set; }

        [JsonPropertyName("show_discount_code_field")]
        public bool ShowDiscountCodeField { get; set; }

        [JsonPropertyName("discount_codes_count")]
        public int DiscountCodesCount { get; set; }

        [JsonPropertyName("account_slug")]
        public string AccountSlug { get; set; }

        [JsonPropertyName("users_count")]
        public int UsersCount { get; set; }

        [JsonPropertyName("releases")]
        public object[] Releases { get; set; }
    }
}
