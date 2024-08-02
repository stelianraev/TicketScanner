namespace CheckIN.Models.TITo.Ticket
{
    using System.Text.Json.Serialization;

    public class TitoTicket
    {
        [JsonPropertyName("_type")]
        public string? Type { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("slug")]
        public string? Slug { get; set; }

        [JsonPropertyName("company_name")]
        public string? CompanyName { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("metadata")]
        public MetaData? MetaData { get; set; }

        [JsonPropertyName("first_name")]
        public string? FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string? LastName { get; set; }

        [JsonPropertyName("name")]
        public string? FullName { get; set; }

        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("phone_number")]
        public string? PhoneNumber { get; set; }

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("reference")]
        public string? Reference { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("test_mode")]
        public string? TestMode { get; set; }

        [JsonPropertyName("registration_id")]
        public int RegistrationId { get; set; }

        public string ReleaseId 
        {
            get
            {
                return ReleaseIdInt.ToString();
            }
        }

        [JsonPropertyName("release_id")]
        public int ReleaseIdInt { get; set; }

        [JsonPropertyName("consented_at")]
        public DateTime? ConsentedAt { get; set; }

        [JsonPropertyName("discount_code_used")]
        public string? DiscountCodeUsed { get; set; }

        [JsonPropertyName("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonPropertyName("responses")]
        public object? Responses { get; set; }

        [JsonPropertyName("assigned")]
        public bool Assigned { get; set; }

        [JsonPropertyName("price_less_tax")]
        public decimal PriceLessTax { get; set; }

        [JsonPropertyName("total_paid")]
        public decimal TotalPaid { get; set; }

        [JsonPropertyName("total_tax_paid")]
        public decimal TotalTaxPaid { get; set; }

        [JsonPropertyName("total_paid_less_tax")]
        public decimal TotalPaidLessTax { get; set; }

        [JsonPropertyName("tags")]
        public string? Tags { get; set; }

        [JsonPropertyName("upgrade_ids")]
        public string[]? UpgradeIds { get; set; }

        [JsonPropertyName("registration_slug")]
        public string? RegistrationSlug { get; set; }

        [JsonPropertyName("release_slug")]
        public string? ReleaseSlug { get; set; }

        [JsonPropertyName("release_title")]
        public string? ReleaseTitle { get; set; }

        [JsonPropertyName("registration")]
        public Registration? Registration { get; set; }

        [JsonPropertyName("release")]
        public Release? Release { get; set; }
    }
}