using Agent.Capabilities;
using OpenAI;
using OpenAI.Chat;

namespace Agent;

public class AgentBuilder
{
    private string? _apiKey;
    private string _model = "gpt-4.1-mini";
    private string _systemPrompt = "You are a helpful assistant.";
    private bool _fileSystemAccessEnabled = true;
    private bool _commandExecutionEnabled = true;
    private IFileSystemAccess? _fileSystem;
    private ICommandExecutor? _commandExecutor;

    public AgentBuilder WithApiKey(string? apiKey)
    {
        _apiKey = apiKey;
        return this;
    }

    public AgentBuilder WithModel(string model)
    {
        _model = model;
        return this;
    }

    public AgentBuilder WithSystemPrompt(string prompt)
    {
        _systemPrompt = prompt;
        return this;
    }

    /// <summary>
    /// Enable or disable file system access (read_file, write_file, list_directory, file_exists).
    /// </summary>
    public AgentBuilder EnableFileSystemAccess(bool enable = true)
    {
        _fileSystemAccessEnabled = enable;
        return this;
    }

    /// <summary>
    /// Enable or disable shell command execution (e.g. ls, cd, pwd).
    /// </summary>
    public AgentBuilder EnableCommandExecution(bool enable = true)
    {
        _commandExecutionEnabled = enable;
        return this;
    }

    /// <summary>
    /// Use a custom file system implementation (interface segregation).
    /// </summary>
    public AgentBuilder WithFileSystem(IFileSystemAccess fileSystem)
    {
        _fileSystem = fileSystem;
        _fileSystemAccessEnabled = true;
        return this;
    }

    /// <summary>
    /// Use a custom command executor implementation (interface segregation).
    /// </summary>
    public AgentBuilder WithCommandExecutor(ICommandExecutor commandExecutor)
    {
        _commandExecutor = commandExecutor;
        _commandExecutionEnabled = true;
        return this;
    }

    public SimpleAgent Build()
    {
        if (string.IsNullOrWhiteSpace(_apiKey))
            throw new InvalidOperationException("API key is required.");

        var client = new OpenAIClient(_apiKey);
        var chatClient = client.GetChatClient(_model);

        var memory = new List<ChatMessage>();
        if (!string.IsNullOrWhiteSpace(_systemPrompt))
            memory.Add(new SystemChatMessage(_systemPrompt));

        // Use default implementations when enabled and not overridden
        var fileSystem = _fileSystem ?? (_fileSystemAccessEnabled ? new FileSystemAccess() : null);
        var commandExecutor = _commandExecutor ?? (_commandExecutionEnabled ? new CommandExecutor() : null);
        var toolExecutor = new ToolExecutor(fileSystem, commandExecutor);

        var tools = AgentTools.CreateTools(_fileSystemAccessEnabled, _commandExecutionEnabled);

        return new SimpleAgent(chatClient, _systemPrompt, memory, toolExecutor, tools);
    }
}
