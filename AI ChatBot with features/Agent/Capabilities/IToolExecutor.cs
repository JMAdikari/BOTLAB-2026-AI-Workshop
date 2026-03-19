namespace Agent.Capabilities;

/// <summary>
/// Executes tool/system messages (e.g. list_directory, read_file, execute_command) by delegating to capabilities.
/// </summary>
public interface IToolExecutor
{
    /// <summary>
    /// Execute a tool by name with JSON arguments; returns result text or error message.
    /// </summary>
    string Execute(string toolName, string argumentsJson);
}
