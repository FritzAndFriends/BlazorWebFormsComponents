using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace BlazorWebFormsComponents.AspxMiddleware;

/// <summary>
/// Parses ASPX markup into a simple AST of AspxNode objects.
/// Pre-processes asp: prefixes and code blocks for safe XML parsing.
/// </summary>
public static partial class AspxParser
{
    private const string ExpressionPlaceholderPrefix = "___ASPX_EXPR_";
    private const string AspPrefix = "asp_";

    /// <summary>
    /// Parse raw ASPX file content into a list of AST nodes plus page metadata.
    /// </summary>
    public static AspxParseResult Parse(string aspxContent)
    {
        var result = new AspxParseResult();
        var content = aspxContent.Trim();

        // Step 1: Extract directives (<%@ ... %>)
        content = ExtractDirectives(content, result);

        // Step 2: Replace server expressions with XML-safe placeholders
        var expressions = new Dictionary<string, ExpressionNode>();
        content = ReplaceExpressions(content, expressions);

        // Step 3: Replace asp: prefix with asp_ so XML parser doesn't choke on namespace
        content = AspOpenTagRegex().Replace(content, "<asp_");
        content = AspCloseTagRegex().Replace(content, "</asp_");

        // Step 4: Wrap in root element and parse as XML
        var xml = $"<__root__>{content}</__root__>";

        XDocument doc;
        try
        {
            doc = XDocument.Parse(xml, LoadOptions.PreserveWhitespace);
        }
        catch (System.Xml.XmlException)
        {
            // If XML parsing fails, treat as raw HTML
            result.Nodes.Add(new HtmlNode { Content = content });
            return result;
        }

        // Step 5: Walk the XML tree and build AST
        if (doc.Root is not null)
        {
            foreach (var node in doc.Root.Nodes())
            {
                var astNode = ConvertXmlNode(node, expressions);
                if (astNode is not null)
                {
                    result.Nodes.Add(astNode);
                }
            }
        }

        return result;
    }

    private static string ExtractDirectives(string content, AspxParseResult result)
    {
        return DirectiveRegex().Replace(content, match =>
        {
            var directive = new DirectiveNode
            {
                DirectiveType = match.Groups[1].Value.Trim()
            };

            var attrMatches = AttributeRegex().Matches(match.Groups[2].Value);
            foreach (Match attrMatch in attrMatches)
            {
                directive.Attributes[attrMatch.Groups[1].Value] = attrMatch.Groups[2].Value;
            }

            result.Directives.Add(directive);

            if (directive.DirectiveType.Equals("Page", StringComparison.OrdinalIgnoreCase))
            {
                if (directive.Attributes.TryGetValue("Title", out var title))
                    result.PageTitle = title;
                if (directive.Attributes.TryGetValue("MasterPageFile", out var master))
                    result.MasterPageFile = master;
            }

            return string.Empty;
        });
    }

    private static string ReplaceExpressions(string content, Dictionary<string, ExpressionNode> expressions)
    {
        var index = 0;
        return ExpressionBlockRegex().Replace(content, match =>
        {
            var prefix = match.Groups[1].Value;
            var code = match.Groups[2].Value.Trim();

            var exprType = prefix switch
            {
                "#" => "databind",
                "$" => "resource",
                "=" => "eval",
                _ => "code"
            };

            var node = new ExpressionNode
            {
                ExpressionType = exprType,
                Expression = code
            };

            var placeholder = $"{ExpressionPlaceholderPrefix}{index++}___";
            expressions[placeholder] = node;

            return $"<!--{placeholder}-->";
        });
    }

    private static AspxNode? ConvertXmlNode(XNode xmlNode, Dictionary<string, ExpressionNode> expressions)
    {
        if (xmlNode is XComment comment)
        {
            var text = comment.Value.Trim();
            if (text.StartsWith(ExpressionPlaceholderPrefix) && expressions.TryGetValue(text, out var expr))
            {
                return expr;
            }
            return new HtmlNode { Content = $"<!--{comment.Value}-->" };
        }

        if (xmlNode is XText textNode)
        {
            var text = textNode.Value;
            if (string.IsNullOrWhiteSpace(text))
                return null;
            return new HtmlNode { Content = text };
        }

        if (xmlNode is XElement element)
        {
            return ConvertXmlElement(element, expressions);
        }

        return null;
    }

    private static AspxNode ConvertXmlElement(XElement element, Dictionary<string, ExpressionNode> expressions)
    {
        var name = element.Name.LocalName;

        // Elements that started as asp: are now asp_ prefixed
        if (name.StartsWith(AspPrefix, StringComparison.OrdinalIgnoreCase))
        {
            return ConvertAspControl(element, name[AspPrefix.Length..], expressions);
        }

        return ConvertHtmlElement(element, expressions);
    }

    private static AspControlNode ConvertAspControl(XElement element, string tagName, Dictionary<string, ExpressionNode> expressions)
    {
        var node = new AspControlNode { TagName = tagName };

        foreach (var attr in element.Attributes())
        {
            node.Attributes[attr.Name.LocalName] = attr.Value;
        }

        foreach (var child in element.Nodes())
        {
            var childNode = ConvertXmlNode(child, expressions);
            if (childNode is not null)
            {
                node.Children.Add(childNode);
            }
        }

        return node;
    }

    private static AspxNode ConvertHtmlElement(XElement element, Dictionary<string, ExpressionNode> expressions)
    {
        // Check if this element contains asp_ children that need decomposition
        var hasAspChildren = element.Descendants()
            .Any(e => e.Name.LocalName.StartsWith(AspPrefix, StringComparison.OrdinalIgnoreCase));
        var hasExprPlaceholders = element.DescendantNodes().OfType<XComment>()
            .Any(c => c.Value.Trim().StartsWith(ExpressionPlaceholderPrefix));

        if (!hasAspChildren && !hasExprPlaceholders)
        {
            // Pure HTML — restore asp: prefix in the output (shouldn't be any, but safe)
            return new HtmlNode { Content = element.ToString(SaveOptions.DisableFormatting) };
        }

        // Has asp: children — decompose into opening tag, children, closing tag
        var children = new List<AspxNode>();

        var openTag = $"<{element.Name.LocalName}";
        foreach (var attr in element.Attributes())
        {
            openTag += $" {attr.Name.LocalName}=\"{attr.Value}\"";
        }
        openTag += ">";
        children.Add(new HtmlNode { Content = openTag });

        foreach (var child in element.Nodes())
        {
            var childNode = ConvertXmlNode(child, expressions);
            if (childNode is not null)
            {
                children.Add(childNode);
            }
        }

        children.Add(new HtmlNode { Content = $"</{element.Name.LocalName}>" });

        // Wrap mixed children in a virtual container
        var wrapper = new AspControlNode
        {
            TagName = "__html_wrapper__",
            Children = children
        };
        return wrapper;
    }

    [GeneratedRegex(@"<%@\s*(\w+)\s+(.*?)%>", RegexOptions.Singleline)]
    private static partial Regex DirectiveRegex();

    [GeneratedRegex(@"(\w+)\s*=\s*""([^""]*)""")]
    private static partial Regex AttributeRegex();

    [GeneratedRegex(@"<%([#$=])?\s*(.*?)%>", RegexOptions.Singleline)]
    private static partial Regex ExpressionBlockRegex();

    [GeneratedRegex(@"<asp:", RegexOptions.IgnoreCase)]
    private static partial Regex AspOpenTagRegex();

    [GeneratedRegex(@"</asp:", RegexOptions.IgnoreCase)]
    private static partial Regex AspCloseTagRegex();
}

/// <summary>
/// Result of parsing an ASPX file.
/// </summary>
public class AspxParseResult
{
    public List<DirectiveNode> Directives { get; set; } = [];
    public List<AspxNode> Nodes { get; set; } = [];
    public string? PageTitle { get; set; }
    public string? MasterPageFile { get; set; }
}
