using DotNetEnv;
using OpenAI.Chat;

Env.TraversePath().Load();

var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY is not set. Add it to the .env file.");

var client = new ChatClient(
    model: "gpt-4.1-mini",
    apiKey: apiKey
);

Console.WriteLine("Simple Bot Online 🤖 (type 'exit' to quit)");

while (true)
{
    Console.Write("You ::: ->>>");
    var userInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userInput)) continue;
    if (userInput.Equals("exit", StringComparison.OrdinalIgnoreCase)) break;

    var response = await client.CompleteChatAsync(userInput);

    Console.WriteLine($"Bot ->> {response.Value.Content[0].Text}");
}
