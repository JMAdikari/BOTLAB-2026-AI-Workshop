namespace Agent.Capabilities;

/// <summary>
/// Shell command execution (e.g. ls, cd, pwd). Segregated interface.
/// </summary>
public interface ICommandExecutor
{
    /// <summary>
    /// Execute a shell command and return stdout; throws on non-zero exit or failure.
    /// </summary>
    string Execute(string commandLine);
}
