using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RDFoxIntegration
{
    public class RDFoxClient
    {
        private readonly HttpClient _client;

        public RDFoxClient(string baseUri, string username, string password)
        {
            _client = new HttpClient { BaseAddress = new Uri(baseUri) };
            var authToken = Encoding.ASCII.GetBytes($"{username}:{password}");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));
        }

        public async Task<string> ListRolesAsync()
        {
            var response = await _client.GetAsync("/roles");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> ListDataStoresAsync()
        {
            var response = await _client.GetAsync("/datastores");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> AddDataAsync(string dataStore, string data)
        {
            var content = new StringContent(data, Encoding.UTF8, "application/sparql-update");
            var response = await _client.PostAsync($"/datastores/{dataStore}/sparql", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> QueryDataAsync(string dataStore, string query)
        {
            var content = new StringContent($"query={Uri.EscapeDataString(query)}", Encoding.UTF8, "application/x-www-form-urlencoded");
            var response = await _client.PostAsync($"/datastores/{dataStore}/sparql", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
