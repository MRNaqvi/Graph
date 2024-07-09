using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace RDFoxIntegration
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var baseUri = "http://localhost:12110";
            var username = "guest"; // Use guest credentials
            var password = "guest"; // Use guest credentials

            var client = new HttpClient();
            var authToken = Encoding.ASCII.GetBytes($"{username}:{password}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(authToken));

            try
            {
                // List roles
                Console.WriteLine("Sending request to list roles...");
                var rolesResponse = await client.GetAsync($"{baseUri}/roles");
                Console.WriteLine($"Response status code: {rolesResponse.StatusCode}");
                if (rolesResponse.IsSuccessStatusCode)
                {
                    var roles = await rolesResponse.Content.ReadAsStringAsync();
                    Console.WriteLine("Roles:");
                    Console.WriteLine(roles);
                }
                else
                {
                    Console.WriteLine($"Failed to list roles. Status: {rolesResponse.StatusCode}");
                    Console.WriteLine(await rolesResponse.Content.ReadAsStringAsync());
                }

                // List data stores
                Console.WriteLine("Sending request to list data stores...");
                var dataStoresResponse = await client.GetAsync($"{baseUri}/datastores");
                Console.WriteLine($"Response status code: {dataStoresResponse.StatusCode}");
                if (dataStoresResponse.IsSuccessStatusCode)
                {
                    var dataStores = await dataStoresResponse.Content.ReadAsStringAsync();
                    Console.WriteLine("Data Stores:");
                    Console.WriteLine(dataStores);
                }
                else
                {
                    Console.WriteLine($"Failed to list data stores. Status: {dataStoresResponse.StatusCode}");
                    Console.WriteLine(await dataStoresResponse.Content.ReadAsStringAsync());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}