using OpenAI.Chat;

namespace Agent;

/// <summary>
/// Defines the tools (list_directory, read_file, write_file, file_exists, execute_command) for the agent.
/// </summary>
public static class AgentTools
{
    public static IReadOnlyList<ChatTool> CreateTools(bool fileSystemEnabled, bool commandExecutionEnabled)
    {
        var list = new List<ChatTool>();

        if (fileSystemEnabled)
        {
            list.Add(ChatTool.CreateFunctionTool(
                functionName: "list_directory",
                functionDescription: "List files and directories at the given path. Use '.' for current directory.",
                functionParameters: Schema("path", "Directory path to list")));

            list.Add(ChatTool.CreateFunctionTool(
                functionName: "read_file",
                functionDescription: "Read the full text content of a file at the given path.",
                functionParameters: Schema("path", "Full or relative path to the file")));

            list.Add(ChatTool.CreateFunctionTool(
                functionName: "write_file",
                functionDescription: "Write text content to a file at the given path. Creates or overwrites the file.",
                functionParameters: BinaryData.FromString("{\"type\":\"object\",\"properties\":{\"path\":{\"type\":\"string\",\"description\":\"File path\"},\"content\":{\"type\":\"string\",\"description\":\"Text content to write\"}},\"required\":[\"path\",\"content\"]}")));

            list.Add(ChatTool.CreateFunctionTool(
                functionName: "file_exists",
                functionDescription: "Check if a file or directory exists at the given path.",
                functionParameters: Schema("path", "Path to check")));
        }

        if (commandExecutionEnabled)
        {
            list.Add(ChatTool.CreateFunctionTool(
                functionName: "execute_command",
                functionDescription: "Run a shell command (e.g. ls, cd, pwd, cat). Use for directory listing, running scripts, or any shell command.",
                functionParameters: Schema("command_line", "The full shell command to execute")));
        }

        return list;
    }

    private static BinaryData Schema(string prop, string description, bool required = true)
    {
        var req = required ? $",\"required\":[\"{prop}\"]" : "";
        return BinaryData.FromString("{\"type\":\"object\",\"properties\":{\"" + prop + "\":{\"type\":\"string\",\"description\":\"" + Escape(description) + "\"}}}" + req + "}");
    }

    private static string Escape(string s) => s.Replace("\\", "\\\\").Replace("\"", "\\\"");
}
