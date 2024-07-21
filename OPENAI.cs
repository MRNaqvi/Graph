 using OpenAI_API;
using OpenAI_API.Chat;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

public class OpenAIClient
{
    private readonly OpenAIAPI _api;

    public OpenAIClient(string apiKey)
    {
        _api = new OpenAIAPI(apiKey);
    }

    private IList<OpenAI_API.Chat.ChatMessage> LoadContext()
    {
        try
        {
            var json = File.ReadAllText(@"C:\Users\defaultuser0\Desktop\chat_context.json");
            var messages = JsonConvert.DeserializeObject<IList<OpenAI_API.Chat.ChatMessage>>(json) ?? new List<OpenAI_API.Chat.ChatMessage>();
            return messages;
        }
        catch (FileNotFoundException)
        {
            return new List<OpenAI_API.Chat.ChatMessage>(); // Return an empty list if no file exists
        }
    }

    public async Task<string> GetExplanationAsync(string prompt)
    {
        // Load context from file
        var messages = LoadContext();

        // Add the user's message to the context
        if (!string.IsNullOrWhiteSpace(prompt))
        {
            messages.Add(new OpenAI_API.Chat.ChatMessage(ChatMessageRole.User, prompt));
        }

        var chatRequest = new ChatRequest
        {
            Model = "gpt-4",
            Messages = messages // Directly use the loaded context with the user's message added
        };

        var result = await _api.Chat.CreateChatCompletionAsync(chatRequest);
        var responseMessage = result.Choices.FirstOrDefault()?.Message.TextContent?.Trim() ?? string.Empty;

        return responseMessage;
    }
}
