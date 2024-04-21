using CheckIN.Configuration;
using CheckIN.Models.TITo;
using Microsoft.Extensions.Options;

namespace CheckIN.Services
{
    public class TiToService : ITiToService
    {
        private static readonly HttpClient _httpClient = new HttpClient();
        private readonly TiToConfiguration _tiToConfiguration;
        public TiToService(IOptions<TiToConfiguration> titoConfiguration)
        {
            _tiToConfiguration = titoConfiguration.Value;
        }
        public async Task<HttpResponseMessage> GetTicket(string ticketId)
        {
            string endpoint = "chk_pK7sbQvPYWAwocdhOMVfrvA/tickets/" + ticketId;
            string url = _tiToConfiguration.BaseUrl + endpoint;

            // Create the HTTP request
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Add authorization header
            request.Headers.Add("Authorization", _tiToConfiguration.Token);
            request.Headers.Add("Accept", "application/json");
            // Send the HTTP request
            var response = await _httpClient.SendAsync(request);

            // Handle the response
            if (response.IsSuccessStatusCode)
            {
                // Process successful response
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response Body: " + responseBody);
            }
            else
            {
                // Handle error response
                Console.WriteLine("Error: " + response.StatusCode);
            }

            return response;
        }

        public async Task<HttpResponseMessage> GetTickets(string checkList)
        {
            // Construct the URL
            string endpoint = "chk_pK7sbQvPYWAwocdhOMVfrvA/tickets";
            string url = _tiToConfiguration.BaseUrl + endpoint;

            // Create the HTTP request
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Add authorization header
            request.Headers.Add("Authorization", _tiToConfiguration.Token);

            // Send the HTTP request
            var response = await _httpClient.SendAsync(request);

            // Handle the response
            if (response.IsSuccessStatusCode)
            {
                // Process successful response
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response Body: " + responseBody);
            }
            else
            {
                // Handle error response
                Console.WriteLine("Error: " + response.StatusCode);
            }

            return response;
        }
    }
}
