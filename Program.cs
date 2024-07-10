using System;
using System.Text;
using System.Threading.Tasks;

namespace RDFoxIntegration
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var baseUri = "http://localhost:12110";
            var username = "guest"; // Use the guest role
            var password = "guest"; // Use the guest role

            var rdfClient = new RDFoxClient(baseUri, username, password);

            while (true)
            {
                Console.WriteLine("Choose an operation:");
                Console.WriteLine("1. List Roles");
                Console.WriteLine("2. List Data Stores");
                Console.WriteLine("3. Insert Data");
                Console.WriteLine("4. Query Data");
                Console.WriteLine("5. Delete Data");
                Console.WriteLine("6. Exit");

                if (int.TryParse(Console.ReadLine(), out int operation))
                {
                    switch (operation)
                    {
                        case 1:
                            await ListRolesAsync(rdfClient);
                            break;
                        case 2:
                            await ListDataStoresAsync(rdfClient);
                            break;
                        case 3:
                            await InsertDataAsync(rdfClient);
                            break;
                        case 4:
                            await QueryDataAsync(rdfClient);
                            break;
                        case 5:
                            await DeleteDataAsync(rdfClient);
                            break;
                        case 6:
                            return;
                        default:
                            Console.WriteLine("Invalid operation.");
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input.");
                }
            }
        }

        private static async Task ListRolesAsync(RDFoxClient rdfClient)
        {
            Console.WriteLine("Sending request to list roles...");
            try
            {
                var roles = await rdfClient.ListRolesAsync();
                Console.WriteLine("Roles:");
                Console.WriteLine(roles);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to list roles. Error: {ex.Message}");
            }
        }

        private static async Task ListDataStoresAsync(RDFoxClient rdfClient)
        {
            Console.WriteLine("Sending request to list data stores...");
            try
            {
                var dataStores = await rdfClient.ListDataStoresAsync();
                Console.WriteLine("Data Stores:");
                Console.WriteLine(dataStores);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to list data stores. Error: {ex.Message}");
            }
        }

        private static async Task InsertDataAsync(RDFoxClient rdfClient)
        {
            Console.WriteLine("Enter the SPARQL update query to insert data (type 'END' on a new line to finish):");
            string data = ReadMultilineInput();
            if (string.IsNullOrEmpty(data))
            {
                Console.WriteLine("No data entered. Operation aborted.");
                return;
            }

            try
            {
                await rdfClient.ExecuteUpdateAsync("myStore", data);
                Console.WriteLine("Data successfully inserted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to insert data. Error: {ex.Message}");
            }
        }

        private static async Task QueryDataAsync(RDFoxClient rdfClient)
        {
            Console.WriteLine("Enter the SPARQL query (type 'END' on a new line to finish):");
            string query = ReadMultilineInput();
            if (string.IsNullOrEmpty(query))
            {
                Console.WriteLine("No query entered. Operation aborted.");
                return;
            }

            try
            {
                var response = await rdfClient.ExecuteQueryAsync("myStore", query);
                Console.WriteLine("Query Data Response:");
                Console.WriteLine(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to query data. Error: {ex.Message}");
            }
        }

        private static async Task DeleteDataAsync(RDFoxClient rdfClient)
        {
            Console.WriteLine("Enter the SPARQL update query to delete data (type 'END' on a new line to finish):");
            string data = ReadMultilineInput();
            if (string.IsNullOrEmpty(data))
            {
                Console.WriteLine("No data entered. Operation aborted.");
                return;
            }

            try
            {
                await rdfClient.ExecuteUpdateAsync("myStore", data);
                Console.WriteLine("Data successfully deleted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete data. Error: {ex.Message}");
            }
        }

        private static string ReadMultilineInput()
        {
            StringBuilder input = new StringBuilder();
            string line;
            while ((line = Console.ReadLine()) != "END")
            {
                input.AppendLine(line);
            }
            return input.ToString();
        }
    }
}

