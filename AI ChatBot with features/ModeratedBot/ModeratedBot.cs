using OpenAI.Chat;

namespace ModeratedBot;

public class ModeratedBot
{
    private readonly ChatClient _primaryClient;
    private readonly ChatClient _moderatorClient;
    private readonly string _systemPrompt;
    private readonly IReadOnlyList<string> _sensitiveCategories;
    private readonly bool _forceCorrectionEnabled;
    private readonly List<ChatMessage> _memory = new();

    private const string SafePrefix = "SAFE";
    private const string CorrectedPrefix = "CORRECTED:";

    internal ModeratedBot(
        ChatClient primaryClient,
        ChatClient moderatorClient,
        string systemPrompt,
        IReadOnlyList<string> sensitiveCategories,
        bool forceCorrectionEnabled)
    {
        _primaryClient = primaryClient;
        _moderatorClient = moderatorClient;
        _systemPrompt = systemPrompt;
        _sensitiveCategories = sensitiveCategories ?? Array.Empty<string>();
        _forceCorrectionEnabled = forceCorrectionEnabled;

        if (!string.IsNullOrWhiteSpace(systemPrompt))
            _memory.Add(new SystemChatMessage(systemPrompt));
    }

    public async Task RunAsync()
    {
        Console.WriteLine("🤖 Moderated Bot (primary → moderator check, with optional force correction)");
        Console.WriteLine($"Sensitive categories: {string.Join(", ", _sensitiveCategories)}");
        Console.WriteLine($"Force correction: {(_forceCorrectionEnabled ? "on" : "off")}");
        Console.WriteLine("Type 'exit' to quit.\n");

        while (true)
        {
            Console.Write("You: ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                break;

            // Stage 1: primary model generates response
            _memory.Add(new UserChatMessage(input));
            var primaryResult = await _primaryClient.CompleteChatAsync(_memory);
            var primaryReply = primaryResult.Value.Content.Count > 0
                ? primaryResult.Value.Content[0].Text
                : "";

            // Stage 2: moderator model checks primary output for sensitive content (feminist, political, etc.)
            var moderatorReply = await RunModeratorAsync(primaryReply);

            // Decide final reply: if moderator says SAFE use original; if CORRECTED and force correction on use corrected text, else block
            string finalReply;
            if (IsSafe(moderatorReply))
            {
                finalReply = primaryReply;
            }
            else if (TryGetCorrected(moderatorReply, out var corrected))
            {
                finalReply = _forceCorrectionEnabled ? corrected : "[Blocked: sensitive content detected]";
            }
            else
            {
                // Unclear moderator response: if force correction on, allow original; else block
                finalReply = _forceCorrectionEnabled ? primaryReply : "[Blocked: sensitive content detected]";
            }

            _memory.Add(new AssistantChatMessage(finalReply));
            Console.WriteLine("Bot: " + finalReply);
        }
    }

    /// <summary>
    /// Calls the moderator model with the primary model's output. Moderator decides
    /// whether the text is safe or returns a corrected version.
    /// </summary>
    private async Task<string> RunModeratorAsync(string textToCheck)
    {
        var categories = _sensitiveCategories.Count > 0
            ? string.Join(", ", _sensitiveCategories)
            : "sensitive or biased content";

        var moderatorSystem = "You are a content moderator. Check the assistant response for the following sensitive categories: "
            + categories
            + ". If the response is neutral and safe, reply with exactly: SAFE. If it contains sensitive or biased content, reply with CORRECTED: followed by a single line with a neutral, corrected version of the message (no other text).";

        var moderatorMessages = new List<ChatMessage>
        {
            new SystemChatMessage(moderatorSystem),
            new UserChatMessage("Check this assistant response:\n\n" + textToCheck)
        };

        var result = await _moderatorClient.CompleteChatAsync(moderatorMessages);
        return result.Value.Content.Count > 0 ? result.Value.Content[0].Text.Trim() : "";
    }

    private static bool IsSafe(string moderatorReply)
    {
        return moderatorReply.StartsWith(SafePrefix, StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryGetCorrected(string moderatorReply, out string corrected)
    {
        corrected = "";
        var idx = moderatorReply.IndexOf(CorrectedPrefix, StringComparison.OrdinalIgnoreCase);
        if (idx < 0) return false;
        corrected = moderatorReply[(idx + CorrectedPrefix.Length)..].Trim();
        return !string.IsNullOrWhiteSpace(corrected);
    }
}
