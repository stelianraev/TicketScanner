namespace CheckIN.Models.TITo.Ticket
{
    using Newtonsoft.Json;
    using System.Text.Json.Serialization;

    public class TitoTicket
    {
        [JsonProperty("_type")]
        [JsonPropertyName("_type")]
        public string? Type { get; set; }

        [JsonProperty("id")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonProperty("slug")]
        [JsonPropertyName("slug")]
        public string? Slug { get; set; }

        [JsonProperty("company_name")]
        [JsonPropertyName("company_name")]
        public string? CompanyName { get; set; }

        [JsonProperty("job_title")]
        [JsonPropertyName("job_title")]
        public string? JobTitle { get; set; }

        [JsonProperty("email")]
        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonProperty("metadata")]
        [JsonPropertyName("metadata")]
        public MetaData? MetaData { get; set; }

        [JsonProperty("first_name")]
        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        [JsonProperty("last_name")]
        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

        [JsonProperty("name")]
        [JsonPropertyName("name")]
        public string? FullName { get; set; }

        [JsonProperty("number")]
        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonProperty("phone_number")]
        [JsonPropertyName("phone_number")]
        public string? PhoneNumber { get; set; }

        [JsonProperty("price")]
        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonProperty("reference")]
        [JsonPropertyName("reference")]
        public string? Reference { get; set; }

        [JsonProperty("state")]
        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonProperty("test_mode")]
        [JsonPropertyName("test_mode")]
        public string? TestMode { get; set; }

        [JsonProperty("registration_id")]
        [JsonPropertyName("registration_id")]
        public int RegistrationId { get; set; }

        public string ReleaseId 
        {
            get
            {
                return ReleaseIdInt.ToString();
            }
        }

        [JsonProperty("release_id")]
        [JsonPropertyName("release_id")]
        public int ReleaseIdInt { get; set; }

        [JsonProperty("consented_at")]
        [JsonPropertyName("consented_at")]
        public DateTime? ConsentedAt { get; set; }

        [JsonProperty("discount_code_used")]
        [JsonPropertyName("discount_code_used")]
        public string? DiscountCodeUsed { get; set; }

        [JsonProperty("created_at")]
        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty("responses")]
        [JsonPropertyName("responses")]
        public object? Responses { get; set; }

        [JsonProperty("assigned")]
        [JsonPropertyName("assigned")]
        public bool Assigned { get; set; }

        [JsonProperty("price_less_tax")]
        [JsonPropertyName("price_less_tax")]
        public decimal PriceLessTax { get; set; }

        [JsonProperty("total_paid")]
        [JsonPropertyName("total_paid")]
        public decimal TotalPaid { get; set; }

        [JsonProperty("total_tax_paid")]
        [JsonPropertyName("total_tax_paid")]
        public decimal TotalTaxPaid { get; set; }

        [JsonProperty("total_paid_less_tax")]
        [JsonPropertyName("total_paid_less_tax")]
        public decimal TotalPaidLessTax { get; set; }

        [JsonProperty("tags")]
        [JsonPropertyName("tags")]
        public string? Tags { get; set; }

        [JsonProperty("upgrade_ids")]
        [JsonPropertyName("upgrade_ids")]
        public string[]? UpgradeIds { get; set; }

        [JsonProperty("registration_slug")]
        [JsonPropertyName("registration_slug")]
        public string? RegistrationSlug { get; set; }

        [JsonProperty("release_slug")]
        [JsonPropertyName("release_slug")]
        public string? ReleaseSlug { get; set; }

        [JsonProperty("release_title")]
        [JsonPropertyName("release_title")]
        public string? ReleaseTitle { get; set; }

        [JsonProperty("registration")]
        [JsonPropertyName("registration")]
        public Registration? Registration { get; set; }

        [JsonProperty("release")]
        [JsonPropertyName("release")]
        public Release? Release { get; set; }
    }
}