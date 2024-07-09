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

                // Query data from the data store
                string query = "SELECT ?s ?p ?o WHERE { ?s ?p ?o }";
                await QueryOperations.QueryDataAsync(rdfClient, "myStore", query);

                // Insert data into the data store
                string insertData = @"
                    PREFIX ex: <http://example.com/>
                    INSERT DATA {
                        ex:subject1 ex:predicate1 ex:object1 .
                    }";
                await InsertOperations.InsertDataAsync(rdfClient, "myStore", insertData);

                // Delete data from the data store
                string deleteData = @"
                    PREFIX ex: <http://example.com/>
                    DELETE WHERE {
                        ex:subject1 ex:predicate1 ex:object1 .
                    }";
                await DeleteOperations.DeleteDataAsync(rdfClient, "myStore", deleteData);

                // Construct data
                string constructQuery = "CONSTRUCT { ?s ?p ?o } WHERE { ?s ?p ?o }";
                await ConstructOperations.ConstructDataAsync(rdfClient, "myStore", constructQuery);
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
    }
}