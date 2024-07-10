using System;
using System.IO;
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
            var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
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

        public async Task<string> ExecuteQueryAsync(string dataStore, string query)
        {
            var response = await _client.GetAsync($"/datastores/{dataStore}/sparql?query={Uri.EscapeDataString(query)}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> ExecuteUpdateAsync(string dataStore, string update)
        {
            var content = new StringContent(update, Encoding.UTF8, "application/sparql-update");
            var response = await _client.PostAsync($"/datastores/{dataStore}/sparql", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task UploadFileAsync(string dataStore, string filePath, string graphName = "")
{
    using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
    using var content = new StreamContent(fileStream);
    content.Headers.ContentType = new MediaTypeHeaderValue("application/x.datalog"); // Adjust as needed

    var requestUri = $"/datastores/{dataStore}/content?operation=add-content";
    if (!string.IsNullOrEmpty(graphName))
    {
        requestUri += $"&default-graph={Uri.EscapeDataString(graphName)}";
    }

    var response = await _client.PatchAsync(requestUri, content);
    response.EnsureSuccessStatusCode();
}

    }
}
