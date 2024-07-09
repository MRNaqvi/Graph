using System;
using System.Threading.Tasks;

namespace RDFoxIntegration
{
    public static class DeleteOperations
    {
        public static async Task DeleteDataAsync(RDFoxClient rdfClient, string dataStore, string data)
        {
            Console.WriteLine("Deleting data from the data store...");
            var response = await rdfClient.ExecuteUpdateAsync(dataStore, data);
            Console.WriteLine("Delete Data Response:");
            Console.WriteLine(response);
        }
    }
}

