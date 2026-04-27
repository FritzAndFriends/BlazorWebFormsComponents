using System.Diagnostics;

namespace BlazorWebFormsComponents.Cli.Interop;

public class PowerShellScriptRunner
{
    public async Task<PowerShellScriptResult> RunAsync(string scriptPath, IReadOnlyList<string> arguments)
    {
        var shell = OperatingSystem.IsWindows() ? "powershell" : "pwsh";
        var quotedArguments = string.Join(" ", arguments.Select(QuoteArgument));
        var psi = new ProcessStartInfo
        {
            FileName = shell,
            Arguments = $"-NoProfile -ExecutionPolicy Bypass -File {QuoteArgument(scriptPath)} {quotedArguments}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process { StartInfo = psi };
        process.Start();

        var stdoutTask = process.StandardOutput.ReadToEndAsync();
        var stderrTask = process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        return new PowerShellScriptResult(
            process.ExitCode,
            await stdoutTask,
            await stderrTask);
    }

    private static string QuoteArgument(string value)
    {
        if (string.IsNullOrEmpty(value))
            return "\"\"";

        if (!value.Contains(' ') && !value.Contains('"'))
            return value;

        return $"\"{value.Replace("\"", "\\\"")}\"";
    }
}

public sealed record PowerShellScriptResult(int ExitCode, string StandardOutput, string StandardError);
