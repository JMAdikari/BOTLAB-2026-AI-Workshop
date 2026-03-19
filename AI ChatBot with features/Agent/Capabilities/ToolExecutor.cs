using System.Text.Json;

namespace Agent.Capabilities;

/// <summary>
/// Dispatches tool calls to IFileSystemAccess and ICommandExecutor.
/// </summary>
public sealed class ToolExecutor : IToolExecutor
{
    private readonly IFileSystemAccess? _fileSystem;
    private readonly ICommandExecutor? _commandExecutor;

    public ToolExecutor(IFileSystemAccess? fileSystem, ICommandExecutor? commandExecutor)
    {
        _fileSystem = fileSystem;
        _commandExecutor = commandExecutor;
    }

    public string Execute(string toolName, string argumentsJson)
    {
        try
        {
            return toolName switch
            {
                "list_directory" => ExecuteListDirectory(argumentsJson),
                "read_file" => ExecuteReadFile(argumentsJson),
                "write_file" => ExecuteWriteFileReturn(argumentsJson),
                "file_exists" => ExecuteFileExists(argumentsJson),
                "execute_command" => ExecuteCommand(argumentsJson),
                _ => $"Unknown tool: {toolName}"
            };
        }
        catch (Exception ex)
        {
            return $"Error: {ex.Message}";
        }
    }

    private string ExecuteListDirectory(string argumentsJson)
    {
        if (_fileSystem == null) return "File system access is disabled.";
        var path = JsonSerializer.Deserialize<JsonElement>(argumentsJson).GetProperty("path").GetString() ?? ".";
        var entries = _fileSystem.ListDirectory(path);
        return string.Join("\n", entries);
    }

    private string ExecuteReadFile(string argumentsJson)
    {
        if (_fileSystem == null) return "File system access is disabled.";
        var path = JsonSerializer.Deserialize<JsonElement>(argumentsJson).GetProperty("path").GetString() ?? "";
        return _fileSystem.ReadFile(path);
    }

    private void ExecuteWriteFile(string argumentsJson)
    {
        if (_fileSystem == null) throw new InvalidOperationException("File system access is disabled.");
        var el = JsonSerializer.Deserialize<JsonElement>(argumentsJson);
        var path = el.GetProperty("path").GetString() ?? "";
        var content = el.GetProperty("content").GetString() ?? "";
        _fileSystem.WriteFile(path, content);
    }

    private string ExecuteWriteFileReturn(string argumentsJson)
    {
        ExecuteWriteFile(argumentsJson);
        return "Written successfully.";
    }

    private string ExecuteFileExists(string argumentsJson)
    {
        if (_fileSystem == null) return "File system access is disabled.";
        var path = JsonSerializer.Deserialize<JsonElement>(argumentsJson).GetProperty("path").GetString() ?? "";
        return _fileSystem.Exists(path) ? "true" : "false";
    }

    private string ExecuteCommand(string argumentsJson)
    {
        if (_commandExecutor == null) return "Command execution is disabled.";
        var commandLine = JsonSerializer.Deserialize<JsonElement>(argumentsJson).GetProperty("command_line").GetString() ?? "";
        return _commandExecutor.Execute(commandLine);
    }
}
