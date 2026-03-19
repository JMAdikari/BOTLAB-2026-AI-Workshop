namespace Agent.Capabilities;

/// <summary>
/// File system operations. Segregated interface for read/write/list.
/// </summary>
public interface IFileSystemAccess
{
    string ReadFile(string path);
    void WriteFile(string path, string content);
    IReadOnlyList<string> ListDirectory(string path);
    bool Exists(string path);
}
