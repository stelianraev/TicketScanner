using Newtonsoft.Json;

namespace CheckIN.Models.TITo
{
    public class Authenticate
    {
        [JsonProperty("authenticated")]
        public bool Authenticated { get; set; }

        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("lookup_mode")]
        public string LookupMode { get; set; }

        [JsonProperty("accounts")]
        public List<string> Accounts { get; set; }

        public string SelectedAccount { get; set; }

        public List<string> Events { get; set; }

        public string SelectedEvent { get; set; }
    }
}