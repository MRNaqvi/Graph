using System;
using System.Threading.Tasks;

namespace RDFoxIntegration
{
    public static class InsertOperations
    {
        public static async Task InsertDataAsync(RDFoxClient rdfClient, string dataStore, string data)
        {
            try
            {
                Console.WriteLine("Inserting data into the data store...");
                await rdfClient.ExecuteUpdateAsync(dataStore, data);
                Console.WriteLine("Insert Data Response: Success");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while inserting data: {ex.Message}");
            }
        }
    }
}

