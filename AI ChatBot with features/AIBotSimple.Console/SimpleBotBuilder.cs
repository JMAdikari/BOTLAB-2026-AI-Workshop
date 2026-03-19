using AIBotSimple.Console.Feature;
using OpenAI;
using OpenAI.Chat;

namespace AIBotSimple.Console;

public class SimpleBotBuilder
{
    private string? _apiKey;
    private string _model = "gpt-4.1-mini";
    private string _systemPrompt = "";
    private bool _enableMemory = true;
    private Personality _personality;
    private List<string>? _sensitiveWords;

    public SimpleBotBuilder WithApiKey(string? apiKey)
    {
        _apiKey = apiKey;
        return this;
    }

    public SimpleBotBuilder WithModel(string model)
    {
        _model = model;
        return this;
    }

    public SimpleBotBuilder WithSystemPrompt(string prompt)
    {
        _systemPrompt = prompt;
        return this;
    }

    public SimpleBotBuilder EnableMemory(bool enable = true)
    {
        _enableMemory = enable;
        return this;
    }

    public SimpleBotBuilder WithPersonality(Personality personality)
    {
        _personality = personality;
        return this;
    }

    public SimpleBotBuilder WithSensitiveWords(List<string> sensitiveWords)
    {
        _sensitiveWords = sensitiveWords;
        return this;
    }

    public SimpleBot Build()
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            throw new InvalidOperationException("API key is required.");

        var client = new OpenAIClient(_apiKey);
        var chatClient = client.GetChatClient(_model);

        var finalPrompt = _personality?.SystemPrompt ?? _systemPrompt;

        var memory = _enableMemory ? new List<ChatMessage>() : new List<ChatMessage>();

        if (!string.IsNullOrWhiteSpace(finalPrompt))
            memory.Add(new SystemChatMessage(finalPrompt));
        else if (!string.IsNullOrWhiteSpace(_personality?.Greeting))
            memory.Add(new SystemChatMessage(_personality.Greeting));

        return new SimpleBot(chatClient, finalPrompt, memory, _sensitiveWords ?? new List<string>());
    }
}