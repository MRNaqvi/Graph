using OpenAI_API;
using OpenAI_API.Chat;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class OpenAIClient
{
    private readonly OpenAIAPI _api;

    public OpenAIClient(string apiKey)
    {
        _api = new OpenAIAPI(apiKey);
    }

    public async Task<string> GetExplanationAsync(List<ChatMessage> messages)
    {
        var chatRequest = new ChatRequest
        {
            Model = "gpt-4",
            Messages = messages
        };

        var result = await _api.Chat.CreateChatCompletionAsync(chatRequest);
        return result.Choices.FirstOrDefault()?.Message.TextContent.Trim() ?? "No explanation provided.";
    }
}

