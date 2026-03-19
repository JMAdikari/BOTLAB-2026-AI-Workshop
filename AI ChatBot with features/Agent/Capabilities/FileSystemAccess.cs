namespace Agent.Capabilities;

/// <summary>
/// Default file system implementation using System.IO.
/// </summary>
public sealed class FileSystemAccess : IFileSystemAccess
{
    public string ReadFile(string path)
    {
        var fullPath = Path.GetFullPath(path);
        return File.ReadAllText(fullPath);
    }

    public void WriteFile(string path, string content)
    {
        var fullPath = Path.GetFullPath(path);
        File.WriteAllText(fullPath, content);
    }

    public IReadOnlyList<string> ListDirectory(string path)
    {
        var fullPath = Path.GetFullPath(path);
        if (!Directory.Exists(fullPath))
            return Array.Empty<string>();
        return Directory.GetFileSystemEntries(fullPath)
            .Select(Path.GetFileName)
            .Where(n => n != null)
            .Cast<string>()
            .ToList();
    }

    public bool Exists(string path)
    {
        var fullPath = Path.GetFullPath(path);
        return File.Exists(fullPath) || Directory.Exists(fullPath);
    }
}
