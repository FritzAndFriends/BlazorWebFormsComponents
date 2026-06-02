namespace BlazorWebFormsComponents.Cli.Interop;

[Obsolete("The CLI no longer shells out to PowerShell. Use native C# services instead.")]
public class PowerShellScriptRunner
{
    public Task<PowerShellScriptResult> RunAsync(string scriptPath, IReadOnlyList<string> arguments)
    {
        throw new NotSupportedException("PowerShell script execution has been removed from the CLI runtime.");
    }
}

public sealed record PowerShellScriptResult(int ExitCode, string StandardOutput, string StandardError);
