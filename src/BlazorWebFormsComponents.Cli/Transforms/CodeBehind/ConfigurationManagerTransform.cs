using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Detects ConfigurationManager.AppSettings and ConfigurationManager.ConnectionStrings
/// patterns, strips the System.Configuration using (BWFC provides the shim in its
/// own namespace), and emits migration guidance.
/// </summary>
public class ConfigurationManagerTransform : ICodeBehindTransform
{
    public string Name => "ConfigurationManager";
    public int Order => 110;

    // using System.Configuration;
    private static readonly Regex SystemConfigUsingRegex = new(
        @"using\s+System\.Configuration;\s*\r?\n?",
        RegexOptions.Compiled);

    // ConfigurationManager.AppSettings["key"]
    private static readonly Regex AppSettingsRegex = new(
        @"ConfigurationManager\.AppSettings\[""([^""]*)""\]",
        RegexOptions.Compiled);

    // ConfigurationManager.ConnectionStrings["name"]
    private static readonly Regex ConnStringsRegex = new(
        @"ConfigurationManager\.ConnectionStrings\[""([^""]*)""\]",
        RegexOptions.Compiled);

    // Broad detection
    private static readonly Regex ConfigManagerRegex = new(
        @"\bConfigurationManager\.(AppSettings|ConnectionStrings)\b",
        RegexOptions.Compiled);

    private static readonly Regex ClassOpenRegex = new(
        @"((?:public|internal|private)\s+(?:partial\s+)?class\s+\w+[^{]*\{)",
        RegexOptions.Compiled);

    private const string GuidanceMarker = "// --- ConfigurationManager Migration ---";

    public string Apply(string content, FileMetadata metadata)
    {
        // Strip comments before detecting ConfigurationManager to avoid matching TODO guidance text
        var codeOnly = Regex.Replace(content, @"//.*$", "", RegexOptions.Multiline);
        var hasConfigManager = ConfigManagerRegex.IsMatch(codeOnly);

        // Strip System.Configuration using — BWFC provides the shim.
        // If the code uses ConfigurationManager, add a using alias to disambiguate from
        // Microsoft.Extensions.Configuration.ConfigurationManager (available via global usings in .NET 10).
        if (SystemConfigUsingRegex.IsMatch(content))
        {
            var replacement = hasConfigManager
                ? "using ConfigurationManager = BlazorWebFormsComponents.ConfigurationManager;\n"
                : "";
            content = SystemConfigUsingRegex.Replace(content, replacement);
        }
        else if (hasConfigManager && !content.Contains("using ConfigurationManager ="))
        {
            // No System.Configuration using but ConfigurationManager is used — add alias at top
            content = "using ConfigurationManager = BlazorWebFormsComponents.ConfigurationManager;\n" + content;
        }

        if (!hasConfigManager) return content;

        // Collect keys for guidance (from code only, not comments)
        var appSettingsKeys = new List<string>();
        foreach (Match m in AppSettingsRegex.Matches(codeOnly))
        {
            var key = m.Groups[1].Value;
            if (!appSettingsKeys.Contains(key)) appSettingsKeys.Add(key);
        }

        var connStringNames = new List<string>();
        foreach (Match m in ConnStringsRegex.Matches(codeOnly))
        {
            var name = m.Groups[1].Value;
            if (!connStringNames.Contains(name)) connStringNames.Add(name);
        }

        // Inject guidance block (idempotent)
        if (!content.Contains(GuidanceMarker) && ClassOpenRegex.IsMatch(content))
        {
            var guidanceBlock = "\n    " + GuidanceMarker + "\n"
                + "    // TODO(bwfc-config): ConfigurationManager calls work via BWFC shim.\n"
                + "    // Ensure app.UseConfigurationManagerShim() is called in Program.cs.\n"
                + (appSettingsKeys.Count > 0
                    ? $"    // AppSettings keys found: {string.Join(", ", appSettingsKeys)}\n"
                    + "    // Add these to appsettings.json under \"AppSettings\" section or as top-level keys.\n"
                    : "")
                + (connStringNames.Count > 0
                    ? $"    // ConnectionString names found: {string.Join(", ", connStringNames)}\n"
                    + "    // Add these to appsettings.json under \"ConnectionStrings\" section.\n"
                    : "");

            content = ClassOpenRegex.Replace(content, "$1" + guidanceBlock, 1);
        }

        return content;
    }
}
