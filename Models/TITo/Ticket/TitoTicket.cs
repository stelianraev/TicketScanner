namespace CheckIN.Models.TITo.Ticket
{
    using Newtonsoft.Json;
    public class TitoTicket
    {
        [JsonProperty("_type")]
        public string Type { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("company_name")]
        public string CompanyName { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("metadata")]
        public MetaData MetaData { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("name")]
        public string FullName { get; set; }

        [JsonProperty("number")]
        public int Number { get; set; }

        [JsonProperty("phone_number")]
        public string PhoneNumber { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("reference")]
        public string Reference { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("test_mode")]
        public string TestMode { get; set; }

        [JsonProperty("registration_id")]
        public int RegistrationId { get; set; }

        [JsonProperty("release_id")]
        public string ReleaseId { get; set; }

        [JsonProperty("consented_at")]
        public DateTime? ConsentedAt { get; set; }

        [JsonProperty("discount_code_used")]
        public string DiscountCodeUsed { get; set; }

        [JsonProperty("created_at")]
        public DateTime? CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty("responses")]
        public string Responses { get; set; }

        [JsonProperty("assigned")]
        public bool Assigned { get; set; }

        [JsonProperty("price_less_tax")]
        public decimal PriceLessTax { get; set; }

        [JsonProperty("total_paid")]
        public decimal TotalPaid { get; set; }

        [JsonProperty("total_tax_paid")]
        public decimal TotalTaxPaid { get; set; }

        [JsonProperty("total_paid_less_tax")]
        public decimal TotalPaidLessTax { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("upgrade_ids")]
        public string[] UpgradeIds { get; set; }

        [JsonProperty("registration_slug")]
        public string RegistrationSlug { get; set; }

        [JsonProperty("release_slug")]
        public string ReleaseSlug { get; set; }

        [JsonProperty("release_title")]
        public string ReleaseTitle { get; set; }

        [JsonProperty("registration")]
        public Registration Registration { get; set; }

        [JsonProperty("release")]
        public Release Release { get; set; }
    }
}