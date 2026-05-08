using System.Text.RegularExpressions;
using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.Markup;

/// <summary>
/// Adds explicit generic type arguments to validator components whose BWFC API
/// requires them. RequiredFieldValidator defaults to the validated control type
/// when it can be inferred and falls back to object only when no type hint exists.
/// </summary>
public class ValidatorGenericTypeTransform : IMarkupTransform
{
    public string Name => "ValidatorGenericType";
    public int Order => 615;

    private static readonly Regex AttributeRegex = new(
        @"\b(?<name>[A-Za-z_:][\w:.-]*)\s*=\s*""(?<value>[^""]*)""",
        RegexOptions.Compiled);

    private static readonly Regex ControlTagRegex = new(
        @"<(?<tag>[A-Za-z][\w]*)\b(?<attributes>[^>]*)/?>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly Regex RequiredFieldValidatorRegex = new(
        @"<RequiredFieldValidator\b(?<attributes>[^>]*?)(?<selfClosing>\s*/?)>",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private static readonly (Regex Pattern, string TypeAttribute, string DefaultType)[] SimpleValidatorPatterns =
    [
        (new Regex(@"<CompareValidator\b(?<attributes>[^>]*?)(?<selfClosing>\s*/?)>", RegexOptions.Compiled | RegexOptions.IgnoreCase), "InputType", "string"),
        (new Regex(@"<RangeValidator\b(?<attributes>[^>]*?)(?<selfClosing>\s*/?)>", RegexOptions.Compiled | RegexOptions.IgnoreCase), "InputType", "string")
    ];

    public string Apply(string content, FileMetadata metadata)
    {
        var controlTypes = BuildControlTypeLookup(content);

        content = RequiredFieldValidatorRegex.Replace(content, match =>
        {
            var attributes = match.Groups["attributes"].Value;
            if (HasAttribute(attributes, "Type"))
            {
                return match.Value;
            }

            var validatorAttributes = ParseAttributes(attributes);
            var validatedControl = validatorAttributes.GetValueOrDefault("ControlToValidate")
                ?? validatorAttributes.GetValueOrDefault("ControlRef");
            var inferredType = !string.IsNullOrWhiteSpace(validatedControl) && controlTypes.TryGetValue(validatedControl, out var controlType)
                ? controlType
                : "object";

            return InsertAttribute(match.Value, "Type", inferredType);
        });

        foreach (var (pattern, typeAttribute, defaultType) in SimpleValidatorPatterns)
        {
            content = pattern.Replace(content, match =>
            {
                if (HasAttribute(match.Groups["attributes"].Value, typeAttribute))
                {
                    return match.Value;
                }

                return InsertAttribute(match.Value, typeAttribute, defaultType);
            });
        }

        return content;
    }

    private static Dictionary<string, string> BuildControlTypeLookup(string content)
    {
        var controlTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        foreach (Match match in ControlTagRegex.Matches(content))
        {
            var tagName = match.Groups["tag"].Value;
            if (tagName.Equals("RequiredFieldValidator", StringComparison.OrdinalIgnoreCase)
                || tagName.Equals("CompareValidator", StringComparison.OrdinalIgnoreCase)
                || tagName.Equals("RangeValidator", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            var attributes = ParseAttributes(match.Groups["attributes"].Value);
            var controlId = attributes.GetValueOrDefault("ID") ?? attributes.GetValueOrDefault("id");
            var inferredType = InferControlType(tagName, attributes);
            if (string.IsNullOrWhiteSpace(controlId) || string.IsNullOrWhiteSpace(inferredType))
            {
                continue;
            }

            controlTypes[controlId] = inferredType;
        }

        return controlTypes;
    }

    private static string? InferControlType(string tagName, IReadOnlyDictionary<string, string> attributes)
    {
        if (tagName.Equals("TextBox", StringComparison.OrdinalIgnoreCase))
        {
            return "string";
        }

        if (attributes.TryGetValue("Type", out var explicitType) && !string.IsNullOrWhiteSpace(explicitType))
        {
            return explicitType;
        }

        if (attributes.TryGetValue("ValueType", out var valueType) && !string.IsNullOrWhiteSpace(valueType))
        {
            return valueType;
        }

        if (attributes.TryGetValue("TValue", out var tValue) && !string.IsNullOrWhiteSpace(tValue))
        {
            return tValue;
        }

        return null;
    }

    private static bool HasAttribute(string attributes, string attributeName) =>
        AttributeRegex.Matches(attributes)
            .Any(match => string.Equals(match.Groups["name"].Value, attributeName, StringComparison.OrdinalIgnoreCase));

    private static Dictionary<string, string> ParseAttributes(string attributes)
    {
        var parsed = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (Match match in AttributeRegex.Matches(attributes))
        {
            parsed[match.Groups["name"].Value] = match.Groups["value"].Value;
        }

        return parsed;
    }

    private static string InsertAttribute(string tag, string attributeName, string attributeValue)
    {
        var firstSpaceIndex = tag.IndexOf(' ');
        if (firstSpaceIndex < 0)
        {
            var closeIndex = tag.IndexOf('>');
            return closeIndex < 0
                ? tag
                : tag.Insert(closeIndex, $" {attributeName}=\"{attributeValue}\"");
        }

        return tag.Insert(firstSpaceIndex, $" {attributeName}=\"{attributeValue}\"");
    }
}
