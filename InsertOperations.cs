using System;
using System.Threading.Tasks;

namespace RDFoxIntegration
{
    public static class InsertOperations
    {
        public static async Task InsertDataAsync(RDFoxClient rdfClient, string dataStore, string data)
        {
            Console.WriteLine("Inserting data into the data store...");
            var response = await rdfClient.ExecuteUpdateAsync(dataStore, data);
            Console.WriteLine("Insert Data Response:");
            Console.WriteLine(response);
        }
    }
}
