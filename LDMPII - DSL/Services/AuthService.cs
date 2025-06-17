using System.Text.Json;
using LDMPII_DSL.ServicesInterfaces;
using LDMPII_Entities.Authentication;
using LDMPII_Helper.CustomExceptions;
using LDMPII_Helper.CustomExceptions.AuthenticationExceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LDMPII_DSL.Services
{
    public class AuthService : IAuthService
    {
        private readonly ILogger<AuthService> _logger;
        private readonly HttpClient _httpClient;
        private readonly TokenCredentials _authRequest;

        public AuthService(ILogger<AuthService> logger, IHttpClientFactory httpClientFactory, IOptions<TokenCredentials> authRequest)
        {
            _logger = logger;
            _authRequest = authRequest.Value;
            if (string.IsNullOrEmpty(_authRequest.TokenUrl))
            {
                throw new ArgumentNullException(nameof(_authRequest.TokenUrl), "Token URL is required in configuration");
            }
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri(authRequest.Value.TokenUrl);
        }

        public async Task<string?> GetTokenAsync()
        {
            var body = new Dictionary<string, string>
            {
                { "grant_type", _authRequest.GrantType },
                { "client_authentication_method", _authRequest.ClientAuthenticationMethod },
                { "client_secret", _authRequest.ClientSecret },
                { "client_id", _authRequest.ClientId }
            };

            using var content = new FormUrlEncodedContent(body);

            try
            {
                var response = await _httpClient.PostAsync("", content);
                response.EnsureSuccessStatusCode();
                _logger.LogInformation("Request Sent Successfully");

                var responseText = await response.Content.ReadAsStringAsync();

                var tokenResponse = await HttpResponseHandler.HandleResponseAsync<TokenResponse>(response, _logger);

                return tokenResponse.AccessToken ?? throw new AuthException("Invalid token response");
            }
            catch (AuthException ex)
            {
                _logger.LogError(ex, "Authentication Failed");
                throw new AuthException("Authentication Unavailable", ex);
            }
            catch (Exception ex) when (ex is not AuthException)
            {
                _logger.LogError(ex, "Unexpected error during authentication");
                throw new AuthException("Authentication service unavailable", ex);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "JSON parsing error");
                throw new AuthException("Failed While Parsing JSON", ex);
            }
        }
    }
}
