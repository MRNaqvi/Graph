using System;
using System.Text;
using System.Threading.Tasks;

namespace RDFoxIntegration
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var baseUri = "http://localhost:12110/";
            var username = "ds-admin"; // Use the guest role
            var password = "raza"; // Use the guest role

            var rdfClient = new RDFoxClient(baseUri, username, password);

            while (true)
            {
                Console.WriteLine("Choose an operation:");
                Console.WriteLine("1. List Roles");
                Console.WriteLine("2. List Data Stores");
                Console.WriteLine("3. Insert Data");
                Console.WriteLine("4. Query Data");
                Console.WriteLine("5. Delete Data");
                Console.WriteLine("6. Add Datalog Rule");
                Console.WriteLine("7. Query Inferred Data");
                Console.WriteLine("8. Upload File");
                Console.WriteLine("9. Exit");

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
                            await AddDatalogRuleAsync(rdfClient);
                            break;
                        case 7:
                            await QueryInferredDataAsync(rdfClient);
                            break;
                        case 8:
                            await UploadFileAsync(rdfClient);
                            break;
                        case 9:
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
                await rdfClient.ExecuteUpdateAsync("ds", data);
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
                var response = await rdfClient.ExecuteQueryAsync("ds", query);
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
                await rdfClient.ExecuteUpdateAsync("ds", data);
                Console.WriteLine("Data successfully deleted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to delete data. Error: {ex.Message}");
            }
        }

        private static async Task AddDatalogRuleAsync(RDFoxClient rdfClient)
        {
            Console.WriteLine("Enter the Datalog rule (type 'END' on a new line to finish):");
            string rule = ReadMultilineInput();
            if (string.IsNullOrEmpty(rule))
            {
                Console.WriteLine("No rule entered. Operation aborted.");
                return;
            }

            string datalogRuleWithPrefix = @"
PREFIX ex: <http://example.org/>

" + rule;

            try
            {
                await rdfClient.ExecuteUpdateAsync("ds", datalogRuleWithPrefix);
                Console.WriteLine("Rule successfully added.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to add rule. Error: {ex.Message}");
            }
        }

        private static async Task QueryInferredDataAsync(RDFoxClient rdfClient)
        {
            string query = @"
PREFIX ex: <http://example.org/>

SELECT ?x ?z
WHERE {
  ?x ex:indirectRelation ?z .
}";

            try
            {
                var response = await rdfClient.ExecuteQueryAsync("ds", query);
                Console.WriteLine("Inferred Data Response:");
                Console.WriteLine(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to query inferred data. Error: {ex.Message}");
            }
        }

        private static async Task UploadFileAsync(RDFoxClient rdfClient)
        {
            Console.WriteLine("Enter the file path to upload:");
            string filePath = Console.ReadLine() ?? string.Empty; // Provide a default value if null
            if (string.IsNullOrEmpty(filePath))
            {
                Console.WriteLine("No file path entered. Operation aborted.");
                return;
            }

            Console.WriteLine("Enter the graph name (or leave empty for default graph):");
            string graphName = Console.ReadLine() ?? string.Empty;

            try
            {
                await rdfClient.UploadFileAsync("ds", filePath, graphName); // Ensure the data store name is correct
                Console.WriteLine("File successfully uploaded.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to upload file. Error: {ex.Message}");
            }
        }

        private static string ReadMultilineInput()
        {
            StringBuilder input = new StringBuilder();
            string line;
            while ((line = Console.ReadLine() ?? "END") != "END")
            {
                input.AppendLine(line);
            }
            return input.ToString().Trim();  // Trim to remove any trailing new lines
        }
    }
}
