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
                Console.WriteLine("Choose an operation:");
                Console.WriteLine("1. List Roles");
                Console.WriteLine("2. List Data Stores");
                Console.WriteLine("3. Insert Data");
                Console.WriteLine("4. Query Data");
                Console.WriteLine("5. Delete All Data");
                Console.Write("Enter the number of the operation: ");
                var choice = Console.ReadLine();

                switch (choice)
                {
                    case "1":
                        await ListRolesAsync(rdfClient);
                        break;
                    case "2":
                        await ListDataStoresAsync(rdfClient);
                        break;
                    case "3":
                        await InsertDataAsync(rdfClient);
                        break;
                    case "4":
                        await QueryDataAsync(rdfClient);
                        break;
                    case "5":
                        await DeleteAllDataAsync(rdfClient);
                        break;
                    default:
                        Console.WriteLine("Invalid choice.");
                        break;
                }
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

        static async Task InsertDataAsync(RDFoxClient rdfClient)
        {
            Console.WriteLine("Enter the data to insert:");
            string insertData = Console.ReadLine();
            await InsertOperations.InsertDataAsync(rdfClient, "myStore", insertData);
        }

        static async Task QueryDataAsync(RDFoxClient rdfClient)
        {
            Console.WriteLine("Enter the SPARQL query:");
            string queryData = Console.ReadLine();
            await QueryOperations.QueryDataAsync(rdfClient, "myStore", queryData);
        }

        static async Task DeleteAllDataAsync(RDFoxClient rdfClient)
        {
            await DeleteOperations.DeleteAllDataAsync(rdfClient, "myStore");
        }
    }
}
