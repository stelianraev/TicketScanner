﻿using CheckIN.Configuration;
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
        public async Task<string> GetTicket(string titoToken, string checkInListId, string ticketId)
        {
            string endpoint = checkInListId + "/tickets/" + ticketId;
            string url = _tiToConfiguration.BaseUrl + endpoint;

            // Create the HTTP request
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // Add authorization header
            request.Headers.Add("Authorization", titoToken);
            request.Headers.Add("Accept", "application/json");
            // Send the HTTP request
            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                //TODO
                // Log the error or handle it accordingly
                return response.StatusCode.ToString();
            }

            var content = await response.Content.ReadAsStringAsync();

            return content;
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