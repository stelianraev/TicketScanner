using Newtonsoft.Json;

namespace CheckIN.Models.TITo.Event
{
    public class EventResponse
    {
        [JsonProperty("_type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("banner")]
        public Banner Banner { get; set; }

        [JsonProperty("banner_url")]
        public string BannerUrl { get; set; }

        [JsonProperty("currency")]
        public string Currency { get; set; }

        [JsonProperty("default_locale")]
        public string DefaultLocale { get; set; }

        [JsonProperty("live")]
        public bool Live { get; set; }

        [JsonProperty("test_mode")]
        public bool TestMode { get; set; }

        [JsonProperty("locales")]
        public List<string> Locales { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("logo")]
        public Logo Logo { get; set; }

        [JsonProperty("private")]
        public bool Private { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("account_id")]
        public string AccountId { get; set; }

        [JsonProperty("start_date")]
        public string StartDate { get; set; }

        [JsonProperty("end_date")]
        public string EndDate { get; set; }

        [JsonProperty("date_or_range")]
        public string DateOrRange { get; set; }

        [JsonProperty("security_token")]
        public string SecurityToken { get; set; }

        [JsonProperty("metadata")]
        public MetaData MetaData { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("setup")]
        public bool Setup { get; set; }

        [JsonProperty("show_discount_code_field")]
        public bool ShowDiscountCodeField { get; set; }

        [JsonProperty("discount_codes_count")]
        public int DiscountCodesCount { get; set; }

        [JsonProperty("account_slug")]
        public string AccountSlug { get; set; }

        [JsonProperty("users_count")]
        public int UsersCount { get; set; }

        [JsonProperty("releases")]
        public object[] Releases { get; set; }
    }
}
