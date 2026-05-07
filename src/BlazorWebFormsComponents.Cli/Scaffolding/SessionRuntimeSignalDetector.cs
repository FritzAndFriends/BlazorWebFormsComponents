using System.Text.RegularExpressions;

namespace BlazorWebFormsComponents.Cli.Scaffolding;

public partial class SessionRuntimeSignalDetector : IRuntimeSignalDetector
{
    public void Apply(string sourcePath, RuntimeProfile profile)
    {
        foreach (var file in RuntimeDetectionFiles.EnumerateFiles(sourcePath, ".cs", ".aspx", ".ascx", ".master"))
        {
            var content = File.ReadAllText(file);
            if (SessionUsageRegex().IsMatch(content))
            {
                profile.NeedsSession = true;
                return;
            }
        }
    }

    [GeneratedRegex(@"\b(Session\s*\[|Context\.Session\b|HttpContext\.Current\.Session\b|Session_Start\s*\()", RegexOptions.Multiline)]
    private static partial Regex SessionUsageRegex();
}
