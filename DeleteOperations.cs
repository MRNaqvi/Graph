using System;
using System.Threading.Tasks;

namespace RDFoxIntegration
{
    public static class DeleteOperations
    {
        public static async Task DeleteAllDataAsync(RDFoxClient rdfClient, string dataStore)
        {
            Console.WriteLine("Deleting all data from the data store...");
            string deleteQuery = @"DELETE WHERE { ?s ?p ?o }";
            await rdfClient.ExecuteUpdateAsync(dataStore, deleteQuery);
            Console.WriteLine("Delete Data Response: Success");
        }
    }
}

