using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Removes <c>[RouteData]</c> parameters from method signatures since the route
/// values are now available as class-level <c>[Parameter]</c> properties
/// (wired by <see cref="RouteParameterWiringTransform"/>).
/// </summary>
public class RouteDataParameterPromotionTransform : ICodeBehindTransform
{
    // Matches [RouteData] type paramName in a parameter list
    // Handles optional whitespace and captures the full decorated param
    private static readonly Regex RouteDataParamRegex = new(
        @"\[RouteData\]\s*\w[\w<>\?\[\]]*\s+\w+",
        RegexOptions.Compiled);

    // Matches the full parameter list of a method that contains [RouteData]
    private static readonly Regex MethodWithRouteDataRegex = new(
        @"(?<before>(?:public|protected|private|internal)\s+(?:(?:static|virtual|override|async)\s+)*\w[\w<>,\s\?\[\]]*\s+\w+\s*\()(?<params>[^)]*\[RouteData\][^)]*)(?<after>\))",
        RegexOptions.Compiled);

    public string Name => "RouteDataParamPromotion";
    public int Order => 136; // Right after RouteParameterWiringTransform (135)

    public string Apply(string content, FileMetadata metadata)
    {
        if (metadata.FileType != FileType.Page)
            return content;

        if (!content.Contains("[RouteData]", StringComparison.Ordinal))
            return content;

        // Process each method that has [RouteData] parameters
        content = MethodWithRouteDataRegex.Replace(content, m =>
        {
            var paramList = m.Groups["params"].Value;

            // Remove [RouteData]-decorated parameters
            var cleanedParams = RouteDataParamRegex.Replace(paramList, "");

            // Clean up resulting commas: ", , " → ", " and leading/trailing commas
            cleanedParams = Regex.Replace(cleanedParams, @",\s*,", ",");
            cleanedParams = Regex.Replace(cleanedParams, @"^\s*,\s*", "");
            cleanedParams = Regex.Replace(cleanedParams, @"\s*,\s*$", "");
            cleanedParams = cleanedParams.Trim();

            return m.Groups["before"].Value + cleanedParams + m.Groups["after"].Value;
        });

        // Remove the RouteData using if no longer needed
        if (!content.Contains("[RouteData]", StringComparison.Ordinal))
        {
            // RouteDataAttribute is in BlazorWebFormsComponents namespace which is likely still needed
            // Don't remove the using
        }

        return content;
    }
}
