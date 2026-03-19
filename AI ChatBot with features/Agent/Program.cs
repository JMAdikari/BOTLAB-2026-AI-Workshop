using Agent;
using DotNetEnv;

class Program
{
    static async Task Main()
    {
        Env.Load();
        var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
            ?? throw new InvalidOperationException("OPENAI_API_KEY is not set. Add it to the .env file.");

        var agent = new AgentBuilder()
            .WithApiKey(apiKey)
            .WithModel("gpt-4.1-mini")
            .WithSystemPrompt("You are a helpful assistant. When the user asks to list files, read/write files, or run commands like ls, cd, or pwd, use the available tools.")
            .EnableFileSystemAccess(true)
            .EnableCommandExecution(true)
            .Build();

        await agent.RunAsync();
    }
}
