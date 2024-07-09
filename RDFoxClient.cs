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

        public async Task<string> CreateRoleAsync(string roleName, string password)
        {
            var content = new StringContent(password, Encoding.UTF8, "text/plain");
            var response = await _client.PostAsync($"/roles/{roleName}", content);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
    }
}
