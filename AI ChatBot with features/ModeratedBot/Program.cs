using ModeratedBot;
using DotNetEnv;

// -----------------------------------------------------------------------------
// ModeratedBot: two-stage flow
// 1. User input → primary model → first response
// 2. That response → moderator model → check for sensitive content (e.g. feminist, political)
// 3. If sensitive and force correction is enabled → use moderator’s corrected text; else block or keep original
// -----------------------------------------------------------------------------

class Program
{
    static async Task Main()
    {
        Env.Load();
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
            ?? throw new InvalidOperationException("OPENAI_API_KEY is not set. Add it to the .env file.");

        // Build the moderated bot using the builder: primary model, moderator model,
        // sensitive categories, and force correction on so the moderator can rewrite when needed.
        var bot = new ModeratedBotBuilder()
            .WithApiKey(apiKey)
            .WithPrimaryModel("gpt-4.1-mini")
            .WithModeratorModel("gpt-4.1-mini")
            .WithSystemPrompt("You are a helpful assistant.")
            .WithSensitiveCategories(new[] { "feminist", "political", "religious" })
            .EnableForceCorrection(false)
            .Build();

        await bot.RunAsync();
    }
}
