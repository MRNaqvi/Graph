using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using VDS.RDF;
using VDS.RDF.Parsing;
using OpenAI_API.Completions;
using OpenAI_API;
using OpenAI_API.Chat;

namespace RDFoxIntegration
{
    public class RDFoxClient
    {
        private readonly HttpClient _client;

        public RDFoxClient(string baseUri, string username, string password)
        {
            _client = new HttpClient { BaseAddress = new Uri(baseUri) };
            var byteArray = Encoding.ASCII.GetBytes($"{username}:{password}");
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        public async Task CreateDataStoreAsync(string dataStoreName)
        {
            var content = new StringContent(string.Empty, Encoding.UTF8, "text/plain");
            var response = await _client.PostAsync($"/datastores/{dataStoreName}?type=parallel-nn", content);

            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Request failed with status code {response.StatusCode}: {responseBody}");
            }
        }

        public async Task CreateRoleAsync(string roleName, string password)
{
    var content = new StringContent(password, Encoding.UTF8, "text/plain");
    var response = await _client.PostAsync($"/roles/{roleName}", content);

    if (!response.IsSuccessStatusCode)
    {
        var responseBody = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"Request failed with status code {response.StatusCode}: {responseBody}");
    }
}

        public async Task GrantPrivilegesAsync(string roleName, string privilege)
        {
            var content = new StringContent($"{{\"privileges\":\"{privilege}\"}}", Encoding.UTF8, "application/json");
            var response = await _client.PatchAsync($"/roles/{roleName}/privileges", content);

            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Request failed with status code {response.StatusCode}: {responseBody}");
            }
        }
        public async Task InitializeDataStoreAsync(string dataStoreName, string adminPassword)
        {
            var commands = new List<string>
            {
                "active ds",
                "role create ds-admin",
                "grant privileges write |roles to ds-admin",
                "grant privileges grant |roles to ds-admin",
                "grant privileges read |roles to ds-admin",
                $"grant privileges full |datastores|{dataStoreName} to ds-admin",
                "grant privileges read |datastores to ds-admin",
                "grant privileges write |datastores to ds-admin",
                "grant privileges grant |datastores to ds-admin",
                $"grant privileges read,write,grant |datastores|{dataStoreName}|rules to ds-admin",
                $"grant privileges read,write,grant |datastores|{dataStoreName}|axioms to ds-admin",
                $"grant privileges read,write,grant |datastores|{dataStoreName}|commitprocedure to ds-admin",
                $"grant privileges read,write,grant |datastores|{dataStoreName}|datasources to ds-admin",
                $"grant privileges read,write,grant |datastores|{dataStoreName}|tupletables to ds-admin",
                $"grant privileges read,write,grant |datastores|{dataStoreName}|namedgraphs|<graph-name> to ds-admin",
                "role show {dataStoreName}-admin"
            };

            foreach (var command in commands)
            {
                var content = new StringContent(command, Encoding.UTF8, "text/plain");
                var response = await _client.PostAsync("/commands", content);
                if (response.IsSuccessStatusCode)
                {   
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Command '{command}' executed successfully as {response.StatusCode}: {responseBody}");
                }
                else
                {
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Command '{command}' failed with status code {response.StatusCode}: {responseBody}");
                }
            }
        }


public async Task<string> ExecuteCommandAsync(string command)
{
    var content = new StringContent(command, Encoding.UTF8, "text/plain");
    var response = await _client.PostAsync("/commands", content);

    if (!response.IsSuccessStatusCode)
    {
        var responseBody = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"Request failed with status code {response.StatusCode}: {responseBody}");
    }

    return await response.Content.ReadAsStringAsync();
}


        
    
public async Task CommitChangesAsync(string dataStoreName)
{
    string command = $"COMMIT {dataStoreName}";
    var response = await _client.GetAsync($"/commands?cmd={Uri.EscapeDataString(command)}");
    if (!response.IsSuccessStatusCode)
    {
        var responseBody = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"Commit failed with status code {response.StatusCode}: {responseBody}");
    }
    Console.WriteLine($"Changes to datastore '{dataStoreName}' committed successfully.");
}



        public async Task<string> ListRolesAsync()
        {
            var response = await _client.GetAsync("/roles");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }
     public async Task<string> ListDataStoresAsync()
        {
            var response = await _client.GetAsync("/datastores");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> ExecuteQueryAsync(string dataStore, string query)
    {
        var response = await _client.GetAsync($"/datastores/{dataStore}/sparql?query={Uri.EscapeDataString(query)}");
        response.EnsureSuccessStatusCode();
        var responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Received response: {responseBody}");
        return responseBody;
    }
        public async Task ExecuteUpdateAsync(string dataStore, string update, string updateType)
        {
            StringContent content;
            HttpResponseMessage response;

            // Check the update type and adjust the request accordingly
            switch (updateType.ToLower())
            {
                case "sparql":
                    content = new StringContent(update, Encoding.UTF8, "application/sparql-update");
                    response = await _client.PostAsync($"/datastores/{dataStore}/sparql", content);
                    break;
                case "datalog":
                    content = new StringContent(update, Encoding.UTF8, "application/x.datalog");
                    response = await _client.PostAsync($"/datastores/{dataStore}/update", content); // Ensure the endpoint is correct
                    break;
                default:
                    throw new ArgumentException("Unsupported update type. Only 'sparql' and 'datalog' are supported.");
            }

            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Request failed with status code {response.StatusCode}: {responseBody}");
            }
        }

        public async Task AddDatalogRuleAsync(string dataStore, string prefix, string rule, string graphName = "")
        {
            // Combine prefix and rule
            string combinedRule = $"{prefix}\n{rule}";

            // Save the combined rule to a file
            string filePath = Path.Combine(@"C:\Users\defaultuser0\Desktop\Test", "rule.dlog");
            string savedFilePath = await SaveRuleToFileAsync(combinedRule, filePath);

            if (savedFilePath != null)
            {
                // Upload the file to RDFox with the correct content type
                await UploadFileAsync(dataStore, savedFilePath, graphName);

                // Optionally, clean up the file after upload
                File.Delete(savedFilePath);
            }
            else
            {
                Console.WriteLine("Failed to save the rule file.");
            }
        }

        public async Task<string> SaveRuleToFileAsync(string rule, string filePath)
        {
            try
            {
                var directory = Path.GetDirectoryName(filePath);
                if (string.IsNullOrEmpty(directory))
                {
                    directory = @"C:\Users\defaultuser0\Desktop\Test";
                }

                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                await File.WriteAllTextAsync(filePath, rule);
                return filePath;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to save the rule to a file.", ex);
            }
        }

        public async Task UploadFileAsync(string dataStore, string filePath, string graphName = "")
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
            using var content = new StreamContent(fileStream);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x.datalog");
            var uri = string.IsNullOrEmpty(graphName)
                ? $"/datastores/{dataStore}/content?operation=add-content"
                : $"/datastores/{dataStore}/content?operation=add-content&default-graph={Uri.EscapeDataString(graphName)}";
            var response = await _client.PatchAsync(uri, content);

            if (!response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                throw new HttpRequestException($"Request failed with status code {response.StatusCode}: {responseBody}");
            }
        }

        public async Task<string> ExplainTriple(string dataStore, string triple)
        {
            string query = $"EXPLAIN {triple}";
            var response = await this.ExecuteQueryAsync(dataStore, query);
            return response;
        }

        public async Task<string> ExplainFactDerivationAsync(string dataStore, string fact, string type = "shortest", int? maxDistanceFromRoot = null, int? maxRuleInstancesPerFact = null)
        {
            var parameters = new Dictionary<string, string>
            {
                {"fact", fact},
                {"type", type}
            };
            if (maxDistanceFromRoot.HasValue)
                parameters.Add("max-distance-from-root", maxDistanceFromRoot.Value.ToString());
            if (maxRuleInstancesPerFact.HasValue)
                parameters.Add("max-rule-instances-per-fact", maxRuleInstancesPerFact.Value.ToString());

            var queryString = string.Join("&", parameters.Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
            var response = await _client.GetAsync($"/datastores/{dataStore}/explanation?{queryString}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public List<Triple> ParseTriplesFromResponse(string responseData)
{
    var triples = new List<Triple>();
    var lines = responseData.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

    // Skip the header if present
    if (lines[0].Trim().StartsWith("?s ?p ?o"))
    {
        lines = lines.Skip(1).ToArray();
    }

    foreach (var line in lines)
    {
        var parts = line.Split(new[] { ' ' }, 3);
        if (parts.Length == 3)
        {
            triples.Add(new Triple(parts[0].Trim('<', '>'), parts[1].Trim('<', '>'), parts[2].Trim('<', '>')));
        }
    }
    return new List<Triple>();
}



        
        public async Task<List<Triple>> FetchTriples(string dataStore)
        {
            string query = "SELECT ?s ?p ?o WHERE {?s ?p ?o}";
            var response = await ExecuteQueryAsync(dataStore, query);
            return ParseTriplesFromResponse(response);
        }

        public async Task<List<Triple>> QueryInferredDataAsync(string dataStore)
        {
            string query = "SELECT ?s ?p ?o WHERE {?s ?p ?o}";
            var response = await ExecuteQueryAsync(dataStore, query);
            return ParseTriplesFromResponse(response);
        }


        public async Task<string> ExplainTripleAsync(string dataStore, string subject, string predicate, string @object)
    {
        string requestUri = $"datastores/{dataStore}/explain?subject={Uri.EscapeDataString(subject)}&predicate={Uri.EscapeDataString(predicate)}&object={Uri.EscapeDataString(@object)}";

        HttpResponseMessage response = await _client.GetAsync(requestUri);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }
    
        public async Task ExplainFactDerivationSpecific(string dataStore, string inferenceToExplain)
        {
            string query = $"EXPLAIN {inferenceToExplain}";
            var response = await ExecuteQueryAsync(dataStore, query);
            Console.WriteLine($"Explanation for {inferenceToExplain}: {response}");
        }
        public async Task UploadSWRLFileAsync(string dataStore, string filePath, string graphName = "")
{
    using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
    using var content = new StreamContent(fileStream);

    // Determine the content type based on the file extension or default to "text/plain"
    string contentType = Path.GetExtension(filePath).ToLower() switch
    {
        ".ttl" => "application/x-turtle",
        ".owl" => "application/owl+xml",
        ".txt" => "text/plain", // Assume text/plain for .txt files
        _ => "application/octet-stream" // Use a general binary type for unknown file types
    };

    content.Headers.ContentType = new MediaTypeHeaderValue(contentType);

    string uri = $"/datastores/{dataStore}/content?operation=add-content";
    if (!string.IsNullOrEmpty(graphName))
    {
        uri += $"&default-graph={Uri.EscapeDataString(graphName)}";
    }

    var response = await _client.PatchAsync(uri, content);
    if (!response.IsSuccessStatusCode)
    {
        var responseBody = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"Request failed with status code {response.StatusCode}: {responseBody}");
    }
}



    








public async Task UploadOWLFileAsync(string dataStore, string filePath, string graphName = "")
    {
        using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        using var content = new StreamContent(fileStream);
        
        // Assuming the contentType is always "application/owl+xml" for .ofn files
        content.Headers.ContentType = new MediaTypeHeaderValue("text/owl-functional");
        
        string uri = string.IsNullOrEmpty(graphName)
            ? $"/datastores/{dataStore}/content?operation=add-content"
            : $"/datastores/{dataStore}/content?operation=add-content&default-graph={Uri.EscapeDataString(graphName)}";
        
        HttpResponseMessage response = await _client.PatchAsync(uri, content);
        if (!response.IsSuccessStatusCode)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException($"Request failed with status code {response.StatusCode}: {responseBody}");
        }
    }

public async Task UploadOwlFileAsync(string dataStore, string filePath, string graphName = "")
{
    // Determine the content type based on file extension
    string contentType = "";
    if (Path.GetExtension(filePath).ToLower() == ".ofn") {
        contentType = "application/owl+xml";  // Assuming RDFox supports this MIME type for OWL Functional Syntax
    } else {
        throw new ArgumentException("Unsupported file format for OWL files.");
    }

    using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
    using var content = new StreamContent(fileStream);
    content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
    var uri = string.IsNullOrEmpty(graphName)
                ? $"/datastores/{dataStore}/content?operation=add-content"
                : $"/datastores/{dataStore}/content?operation=add-content&default-graph={Uri.EscapeDataString(graphName)}";
    var response = await _client.PatchAsync(uri, content);

    if (!response.IsSuccessStatusCode)
    {
        var responseBody = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"Request failed with status code {response.StatusCode}: {responseBody}");
    }
}





        public async Task UploadTTLFileAsync(string dataStore, string filePath, string graphName = "")
{
    string contentType = "text/turtle"; // MIME type for Turtle files
    using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
    using var content = new StreamContent(fileStream);
    content.Headers.ContentType = new MediaTypeHeaderValue(contentType); // Set content type
    var uri = string.IsNullOrEmpty(graphName)
                ? $"/datastores/{dataStore}/content?operation=add-content"
                : $"/datastores/{dataStore}/content?operation=add-content&default-graph={Uri.EscapeDataString(graphName)}";
    var response = await _client.PatchAsync(uri, content);

    if (!response.IsSuccessStatusCode)
    {
        var responseBody = await response.Content.ReadAsStringAsync();
        throw new HttpRequestException($"Request failed with status code {response.StatusCode}: {responseBody}");
    }
    else
    {
        Console.WriteLine("TTL file successfully uploaded.");
    }
}


    



    }
}
