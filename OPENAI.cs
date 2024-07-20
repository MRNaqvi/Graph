using OpenAI_API;
using OpenAI_API.Chat;

public class OpenAIClient
{
    private readonly OpenAIAPI _api;

    public OpenAIClient(string apiKey)
    {
        _api = new OpenAIAPI(apiKey);
    }

    public async Task<string> GetExplanationAsync(string prompt)
    {
        var chatRequest = new ChatRequest
        {
            Model = "gpt-4",
            Messages = new List<ChatMessage>
            {
                new ChatMessage(ChatMessageRole.System, "You are a helpful assistant that explains fact derivations with counterfactual, commonsense explanations remember only precisie and consice."),
                new ChatMessage(ChatMessageRole.User, prompt)
            }
        };

        var result = await _api.Chat.CreateChatCompletionAsync(chatRequest);
        return result.Choices.FirstOrDefault()?.Message.TextContent.Trim() ?? "No explanation provided.";
    }
}
