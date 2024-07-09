using System;
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

            var rdfClient = new RDFoxClient(baseUri, username, password);

            try
            {
                // List roles
                await ListRolesAsync(rdfClient);

                // List data stores
                await ListDataStoresAsync(rdfClient);

                // Add data to the data store
                await AddDataAsync(rdfClient);

                // Query data from the data store
                await QueryDataAsync(rdfClient);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }

        static async Task ListRolesAsync(RDFoxClient rdfClient)
        {
            Console.WriteLine("Sending request to list roles...");
            var roles = await rdfClient.ListRolesAsync();
            Console.WriteLine("Roles:");
            Console.WriteLine(roles);
        }

        static async Task ListDataStoresAsync(RDFoxClient rdfClient)
        {
            Console.WriteLine("Sending request to list data stores...");
            var dataStores = await rdfClient.ListDataStoresAsync();
            Console.WriteLine("Data Stores:");
            Console.WriteLine(dataStores);
        }

        static async Task AddDataAsync(RDFoxClient rdfClient)
        {
            Console.WriteLine("Adding data to the data store...");
            var data = @"
            PREFIX ex: <http://example.com/>
            INSERT DATA {
                ex:subject1 ex:predicate1 ex:object1 .
            }";
            var response = await rdfClient.AddDataAsync("myStore", data);
            Console.WriteLine("Add Data Response:");
            Console.WriteLine(response);
        }

        static async Task QueryDataAsync(RDFoxClient rdfClient)
        {
            Console.WriteLine("Querying data from the data store...");
            var query = "SELECT ?s ?p ?o WHERE { ?s ?p ?o }";
            var response = await rdfClient.QueryDataAsync("myStore", query);
            Console.WriteLine("Query Result:");
            Console.WriteLine(response);
        }
    }
}