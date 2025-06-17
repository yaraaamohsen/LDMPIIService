using System.Net;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace LDMPII_Helper.CustomExceptions
{
    public class HttpResponseHandler
    {
        public static async Task<T> HandleResponseAsync<T>(HttpResponseMessage response, ILogger _logger)
        {
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("HTTP Error - Status: {StatusCode}, Content: {Content}", response.StatusCode, errorContent);
                throw response.StatusCode switch
                {
                    HttpStatusCode.Unauthorized => new UnauthorizedAccessException("Authentication Failed"),
                    HttpStatusCode.NotFound => new KeyNotFoundException("Resource Not Found"),
                    HttpStatusCode.BadRequest => new ArgumentException("Bad Request" + errorContent),
                    _ => new HttpRequestException($"HTTP Error: {response.StatusCode} - {errorContent}")
                };
            }
            return await response.Content.ReadFromJsonAsync<T>();
        }
    }
}
