using AIBotSimple.Console;
using AIBotSimple.Console.Feature;
using DotNetEnv;

class Program
{
    static async Task Main()
    {
        Env.Load();
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
            ?? throw new InvalidOperationException("OPENAI_API_KEY is not set. Add it to the .env file.");

        var friendlyPersonality = new Personality(
            name: "FriendlyBot",
            greeting: "Hi! I’m FriendlyBot 🤖",
            style: "friendly",
            systemPrompt: "You are a friendly and cheerful assistant that helps users with a positive tone."
        );

        var sensitiveWords = new List<string>
        {
            "Fuck", "Kill"
        };

        var bot = new SimpleBotBuilder()
            .WithApiKey(apiKey)
            .WithModel("gpt-4.1-mini")
            .WithSystemPrompt("You are a helpful assistant.")
            .EnableMemory(false)
            .WithPersonality(friendlyPersonality)
            .WithSensitiveWords(sensitiveWords)
            .Build();

        await bot.RunAsync();
    }
}