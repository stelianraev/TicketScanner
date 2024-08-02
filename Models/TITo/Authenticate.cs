using System.Text.Json.Serialization;

namespace CheckIN.Models.TITo
{
    public class Authenticate
    {
        [JsonPropertyName("authenticated")]
        public bool Authenticated { get; set; }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("lookup_mode")]
        public string LookupMode { get; set; }

        [JsonPropertyName("accounts")]
        public List<string> Accounts { get; set; }

        public string SelectedAccount { get; set; }

        public List<string> Events { get; set; }

        public string SelectedEvent { get; set; }
    }
}