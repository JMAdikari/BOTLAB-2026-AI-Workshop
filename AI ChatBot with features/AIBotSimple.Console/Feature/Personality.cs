namespace AIBotSimple.Console.Feature;

public class Personality
{
    public string Name { get; set; } = "Default";
    public string Greeting { get; set; } = "Hello!";
    public string Style { get; set; } = "friendly";
    public string SystemPrompt { get; set; } = "";

    public Personality(string name = "Default", string greeting = "Hello!", string style = "friendly", string systemPrompt = "")
    {
        Name = name;
        Greeting = greeting;
        Style = style;
        SystemPrompt = systemPrompt;
    }
}