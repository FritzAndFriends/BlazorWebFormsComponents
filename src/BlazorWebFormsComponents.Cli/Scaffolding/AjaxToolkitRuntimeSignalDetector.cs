using System.Text.RegularExpressions;

namespace BlazorWebFormsComponents.Cli.Scaffolding;

public partial class AjaxToolkitRuntimeSignalDetector : IRuntimeSignalDetector
{
    public void Apply(string sourcePath, RuntimeProfile profile)
    {
        foreach (var file in RuntimeDetectionFiles.EnumerateFiles(sourcePath, ".aspx", ".ascx", ".master"))
        {
            var content = File.ReadAllText(file);
            if (AjaxToolkitPrefixRegex().IsMatch(content))
            {
                profile.NeedsAjaxToolkit = true;
                return;
            }
        }
    }

    [GeneratedRegex(@"<ajaxToolkit:", RegexOptions.IgnoreCase)]
    private static partial Regex AjaxToolkitPrefixRegex();
}
