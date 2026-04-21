using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Adds BWFC Identity type aliases or namespace using to code-behind files that reference
/// BWFC Identity shim types. Uses per-type aliases for types that conflict with
/// Microsoft.AspNetCore.Identity (IdentityUser, IdentityResult, UserLoginInfo, IdentityDbContext)
/// and a namespace using for BWFC-only types (ApplicationUserManager, etc.).
/// </summary>
public class IdentityUsingTransform : ICodeBehindTransform
{
    public string Name => "IdentityUsing";
    public int Order => 103; // After UsingStripTransform (100), before EntityFrameworkTransform (105)

    // Types that exist in BOTH BlazorWebFormsComponents.Identity AND Microsoft.AspNetCore.Identity
    // These need per-type aliases to avoid CS0104 ambiguity
    private static readonly Dictionary<string, string> ConflictingTypes = new()
    {
        ["IdentityUser"] = "using IdentityUser = BlazorWebFormsComponents.Identity.IdentityUser;",
        ["IdentityResult"] = "using IdentityResult = BlazorWebFormsComponents.Identity.IdentityResult;",
        ["UserLoginInfo"] = "using UserLoginInfo = BlazorWebFormsComponents.Identity.UserLoginInfo;",
    };

    // BWFC-only Identity shim types — no conflict, namespace using is safe
    // IdentityDbContext is generic (IdentityDbContext<TUser>) so C# can't alias open generics;
    // it also lives in a different namespace (Microsoft.AspNetCore.Identity.EntityFrameworkCore)
    // than the other conflicting types, so namespace using is safe for it.
    private static readonly string[] BwfcOnlyTypes =
    {
        "ApplicationUserManager", "ApplicationSignInManager", "SignInStatus",
        "DefaultAuthenticationTypes", "IdentityDbContext"
    };

    // Fully-qualified Microsoft.AspNetCore.Identity references (these should NOT trigger BWFC aliases)
    private static readonly Regex FullyQualifiedIdentityRegex = new(
        @"Microsoft\.AspNetCore\.Identity\.\w+",
        RegexOptions.Compiled);

    private static readonly Regex UsingBlazorIdentityRegex = new(
        @"using\s+BlazorWebFormsComponents\.Identity;",
        RegexOptions.Compiled);

    public string Apply(string content, FileMetadata metadata)
    {
        // If namespace using is already present, nothing to do
        if (UsingBlazorIdentityRegex.IsMatch(content))
            return content;

        var aliasLines = new List<string>();
        var needsNamespaceUsing = false;

        // Check for conflicting types — these get per-type aliases
        foreach (var (typeName, aliasLine) in ConflictingTypes)
        {
            var typeRegex = new Regex($@"\b{typeName}\b");
            if (!typeRegex.IsMatch(content))
                continue;

            // Skip if all occurrences are fully-qualified as Microsoft.AspNetCore.Identity
            var fqRegex = new Regex($@"Microsoft\.AspNetCore\.Identity\.{typeName}\b");
            var totalMatches = typeRegex.Matches(content).Count;
            var fqMatches = fqRegex.Matches(content).Count;
            if (fqMatches >= totalMatches)
                continue;

            // Check if a type alias for this type already exists
            var aliasCheck = new Regex($@"using\s+{typeName}\s*=");
            if (aliasCheck.IsMatch(content))
                continue;

            aliasLines.Add(aliasLine);
        }

        // Check for BWFC-only types — these can use namespace using
        foreach (var typeName in BwfcOnlyTypes)
        {
            var typeRegex = new Regex($@"\b{typeName}\b");
            if (typeRegex.IsMatch(content))
            {
                needsNamespaceUsing = true;
                break;
            }
        }

        if (aliasLines.Count == 0 && !needsNamespaceUsing)
            return content;

        // Build the insertion block
        var insertBlock = "";
        if (needsNamespaceUsing)
            insertBlock += "using BlazorWebFormsComponents.Identity;\n";
        foreach (var alias in aliasLines.OrderBy(a => a))
            insertBlock += alias + "\n";

        // Insert after the last existing using directive
        var lastUsing = Regex.Match(content, @"^using\s+[^;]+;\s*$", RegexOptions.Multiline | RegexOptions.RightToLeft);
        if (lastUsing.Success)
        {
            var insertPos = lastUsing.Index + lastUsing.Length;
            content = content[..insertPos] + "\n" + insertBlock + content[insertPos..];
        }
        else
        {
            content = insertBlock + content;
        }

        return content;
    }
}
