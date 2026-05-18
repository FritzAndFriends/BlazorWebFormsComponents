using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Fixes null-safety issues with Session.GetString() calls.
/// Session.GetString("key") returns null when the key doesn't exist,
/// so .ToString() on the result causes NRE.
/// Rewrites: .GetString(X).ToString() → .GetString(X) ?? string.Empty
/// Also adds null-conditional: .GetString(X).SomeMethod → .GetString(X)?.SomeMethod
/// </summary>
public class SessionGetStringNullSafetyTransform : ICodeBehindTransform
{
    public string Name => "SessionGetStringNullSafety";
    public int Order => 405; // After SessionDetectTransform (400)

    // Matches .GetString("key").ToString() or .GetString(variable).ToString()
    private static readonly Regex GetStringToStringRegex = new(
        @"\.GetString\((?<arg>[^)]+)\)\.ToString\(\)",
        RegexOptions.Compiled);

    // Matches .GetString("key").SomeMember (not already ?.)
    private static readonly Regex GetStringMemberAccessRegex = new(
        @"\.GetString\((?<arg>[^)]+)\)(?<!\?)\.(?<member>[A-Z]\w*)",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        if (!content.Contains(".GetString(", StringComparison.Ordinal))
            return content;

        // Replace .GetString(X).ToString() with .GetString(X) ?? string.Empty
        var modified = GetStringToStringRegex.Replace(content,
            ".GetString(${arg}) ?? string.Empty");

        // Replace .GetString(X).Member with .GetString(X)?.Member (null-conditional)
        modified = GetStringMemberAccessRegex.Replace(modified,
            ".GetString(${arg})?.${member}");

        return modified;
    }
}
