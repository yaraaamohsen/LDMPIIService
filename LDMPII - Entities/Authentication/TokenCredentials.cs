using System.Text.Json.Serialization;

namespace LDMPII_Entities.Authentication
{
    public class TokenCredentials
    {
        [JsonPropertyName("client_authentication_method")]
        public string ClientAuthenticationMethod { get; set; }
        [JsonPropertyName("grant_type")]
        public string GrantType { get; set; }
        [JsonPropertyName("client_id")]
        public string ClientId { get; set; }
        [JsonPropertyName("client_secret")]
        public string ClientSecret { get; set; }
        public string TokenUrl { get; set; }
    }
}
