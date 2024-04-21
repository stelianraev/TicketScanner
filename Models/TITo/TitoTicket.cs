using System.Text.Json.Serialization;

namespace CheckIN.Models.TITo
{
    public class TitoTicket
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("first_name")]
        public string FirstName { get; set; }

        [JsonPropertyName("last_name")]
        public string LastName { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("phone_number")]
        public int PhoneNumber { get; set; }

        [JsonPropertyName("company_name")]
        public string CompanyNme { get; set; }

        [JsonPropertyName("release_title")]
        public string ReleaseTitle { get; set; }

        [JsonPropertyName("reference")]
        public string Reference { get; set; }

        [JsonPropertyName("registration_reference")]
        public string RegistrationReference { get; set; }

        [JsonPropertyName("tags")]
        public string Tags { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public string UpdatedAt { get; set; }
    }
}