using System.Diagnostics;

namespace Agent.Capabilities;

/// <summary>
/// Executes shell commands (e.g. ls, cd, pwd) via the current process shell.
/// </summary>
public sealed class CommandExecutor : ICommandExecutor
{
    public string Execute(string commandLine)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = GetShell(),
            Arguments = GetShellArgs(commandLine),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        if (process == null)
            throw new InvalidOperationException("Failed to start process.");

        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0 && !string.IsNullOrWhiteSpace(stderr))
            return $"exit code {process.ExitCode}\nstderr: {stderr}\nstdout: {stdout}";

        return stdout.TrimEnd();
    }

    private static string GetShell()
    {
        if (OperatingSystem.IsWindows())
            return "cmd.exe";
        return "/bin/bash";
    }

    private static string GetShellArgs(string commandLine)
    {
        if (OperatingSystem.IsWindows())
            return $"/c \"{commandLine.Replace("\"", "\\\"")}\"";
        return $"-c \"{commandLine.Replace("\"", "\\\"")}\"";
    }
}
