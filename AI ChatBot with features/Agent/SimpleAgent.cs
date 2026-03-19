using Agent.Capabilities;
using OpenAI.Chat;

namespace Agent;

public class SimpleAgent
{
    private readonly ChatClient _chatClient;
    private readonly List<ChatMessage> _memory;
    private readonly string _systemPrompt;
    private readonly IToolExecutor _toolExecutor;
    private readonly IReadOnlyList<ChatTool> _tools;

    internal SimpleAgent(
        ChatClient chatClient,
        string systemPrompt,
        List<ChatMessage> memory,
        IToolExecutor toolExecutor,
        IReadOnlyList<ChatTool> tools)
    {
        _chatClient = chatClient;
        _systemPrompt = systemPrompt;
        _memory = memory;
        _toolExecutor = toolExecutor;
        _tools = tools;
    }

    public async Task RunAsync()
    {
        System.Console.WriteLine("🤖 Simple Agent");
        System.Console.WriteLine("Capabilities: file system and command execution (ls, cd, pwd, etc.)");
        System.Console.WriteLine("Type 'exit' to quit.\n");

        while (true)
        {
            System.Console.Write("You: ");
            var input = System.Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            if (input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                break;

            _memory.Add(new UserChatMessage(input));

            var options = new ChatCompletionOptions
            {
                Tools = { }
            };
            foreach (var tool in _tools)
                options.Tools.Add(tool);

            bool requiresAction;
            do
            {
                requiresAction = false;
                var result = await _chatClient.CompleteChatAsync(_memory, options);
                var completion = result.Value;

                switch (completion.FinishReason)
                {
                    case ChatFinishReason.Stop:
                        var text = completion.Content.Count > 0 ? completion.Content[0].Text : "";
                        _memory.Add(new AssistantChatMessage(text));
                        System.Console.WriteLine("Agent: " + text);
                        break;

                    case ChatFinishReason.ToolCalls:
                        _memory.Add(new AssistantChatMessage(completion));
                        foreach (var toolCall in completion.ToolCalls)
                        {
                            var args = toolCall.FunctionArguments?.ToString() ?? "{}";
                            var toolResult = _toolExecutor.Execute(toolCall.FunctionName, args);
                            _memory.Add(new ToolChatMessage(toolCall.Id, toolResult));
                        }
                        requiresAction = true;
                        break;

                    case ChatFinishReason.Length:
                        System.Console.WriteLine("Agent: (Response truncated.)");
                        break;

                    default:
                        var fallback = completion.Content.Count > 0 ? completion.Content[0].Text : completion.FinishReason.ToString();
                        _memory.Add(new AssistantChatMessage(fallback));
                        System.Console.WriteLine("Agent: " + fallback);
                        break;
                }
            } while (requiresAction);
        }
    }
}
