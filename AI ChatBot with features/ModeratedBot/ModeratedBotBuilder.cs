using OpenAI;
using OpenAI.Chat;

namespace ModeratedBot;

/// <summary>
/// Builder for a two-stage bot: primary model generates a response,
/// then moderator model checks that response for sensitive content (e.g. feminist, political)
/// and can force-correct it when enabled.
/// </summary>
public class ModeratedBotBuilder
{
    private string? _apiKey;
    private string _primaryModel = "gpt-4.1-mini";
    private string _moderatorModel = "gpt-4.1-mini";
    private string _systemPrompt = "You are a helpful assistant.";
    private IReadOnlyList<string> _sensitiveCategories = new[] { "feminist", "political" };
    private bool _forceCorrectionEnabled = true;

    /// <summary>Set the OpenAI API key (used for both primary and moderator models).</summary>
    public ModeratedBotBuilder WithApiKey(string? apiKey)
    {
        _apiKey = apiKey;
        return this;
    }

    /// <summary>Model used to generate the initial response to the user.</summary>
    public ModeratedBotBuilder WithPrimaryModel(string model)
    {
        _primaryModel = model;
        return this;
    }

    /// <summary>Model used to check the primary output for sensitive content and optionally correct it.</summary>
    public ModeratedBotBuilder WithModeratorModel(string model)
    {
        _moderatorModel = model;
        return this;
    }

    /// <summary>System prompt for the primary (response) model.</summary>
    public ModeratedBotBuilder WithSystemPrompt(string prompt)
    {
        _systemPrompt = prompt;
        return this;
    }

    /// <summary>Categories the moderator checks for (e.g. "feminist", "political").</summary>
    public ModeratedBotBuilder WithSensitiveCategories(IReadOnlyList<string> categories)
    {
        _sensitiveCategories = categories ?? Array.Empty<string>();
        return this;
    }

    /// <summary>When true, if the moderator finds sensitive content it returns a corrected version; when false, the reply is blocked.</summary>
    public ModeratedBotBuilder EnableForceCorrection(bool enable = true)
    {
        _forceCorrectionEnabled = enable;
        return this;
    }

    public ModeratedBot Build()
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            throw new InvalidOperationException("API key is required.");

        var client = new OpenAIClient(_apiKey);
        var primaryChat = client.GetChatClient(_primaryModel);
        var moderatorChat = client.GetChatClient(_moderatorModel);

        return new ModeratedBot(
            primaryChat,
            moderatorChat,
            _systemPrompt,
            _sensitiveCategories,
            _forceCorrectionEnabled
        );
    }
}
