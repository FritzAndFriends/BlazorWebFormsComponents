using System.Text.RegularExpressions;
using AngleSharp.Html.Parser;
using AngleSharp.Dom;

namespace BlazorWebFormsComponents.AspxMiddleware;

/// <summary>
/// Parses ASPX markup into a simple AST of AspxNode objects.
/// Uses AngleSharp for tolerant HTML parsing. Pre-processes directives and expressions.
/// </summary>
public static partial class AspxParser
{
    private const string ExpressionPlaceholderPrefix = "___ASPX_EXPR_";
    private const string AspPrefix = "asp_";
    private const string CommentPlaceholderPrefix = "___ASPX_COMMENT_";

    /// <summary>
    /// Parse raw ASPX file content into a list of AST nodes plus page metadata.
    /// </summary>
    public static AspxParseResult Parse(string aspxContent)
    {
        var result = new AspxParseResult();
        var content = aspxContent.Trim();

        // Step 1: Extract directives (<%@ ... %>)
        content = ExtractDirectives(content, result);

        // Step 2: Strip server-side comments (<%-- --%>)
        var comments = new Dictionary<string, string>();
        content = StripServerComments(content, comments);

        // Step 3: Replace server expressions with HTML-safe placeholders
        var expressions = new Dictionary<string, ExpressionNode>();
        content = ReplaceExpressions(content, expressions);

        // Step 4: Build case mapping before lowercasing (AngleSharp lowercases tag names)
        var caseMapping = BuildCaseMapping(content);

        // Step 5: Replace asp: prefix with asp_ (AngleSharp treats colons specially)
        content = AspOpenTagRegex().Replace(content, "<asp_");
        content = AspCloseTagRegex().Replace(content, "</asp_");

        // Step 6: Convert self-closing asp_ tags to explicitly closed tags
        // (AngleSharp treats unknown self-closing tags as open tags in HTML5 mode)
        content = SelfClosingAspTagRegex().Replace(content, match =>
        {
            var tagName = match.Groups[1].Value;
            var attributes = match.Groups[2].Value;
            return $"<{tagName}{attributes}></{tagName}>";
        });

        // Step 7: Parse with AngleSharp (tolerant HTML5 parser)
        var parser = new HtmlParser();
        var document = parser.ParseDocument(content);

        // Step 8: Walk the DOM tree and build AST
        if (document.Body is not null)
        {
            foreach (var child in document.Body.ChildNodes)
            {
                var astNode = ConvertDomNode(child, expressions, caseMapping);
                if (astNode is not null)
                {
                    result.Nodes.Add(astNode);
                }
            }
        }

        return result;
    }

    private static Dictionary<string, string> BuildCaseMapping(string content)
    {
        // Extract original case for asp: tags before AngleSharp lowercases them
        var mapping = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var matches = AspTagRegex().Matches(content);
        foreach (Match match in matches)
        {
            var tagName = match.Groups[1].Value;
            // Store lowercase -> original case
            mapping[tagName.ToLowerInvariant()] = tagName;
        }
        return mapping;
    }

    private static string ExtractDirectives(string content, AspxParseResult result)
    {
        return DirectiveRegex().Replace(content, match =>
        {
            var directive = new DirectiveNode
            {
                DirectiveType = match.Groups[1].Value.Trim()
            };

            // Support both double-quote and single-quote attributes
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

    private static string StripServerComments(string content, Dictionary<string, string> comments)
    {
        var index = 0;
        return ServerCommentRegex().Replace(content, match =>
        {
            var placeholder = $"<!--{CommentPlaceholderPrefix}{index++}___-->";
            comments[placeholder] = match.Value;
            // Return empty string to strip comments entirely
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

    private static AspxNode? ConvertDomNode(INode domNode, Dictionary<string, ExpressionNode> expressions, Dictionary<string, string> caseMapping)
    {
        if (domNode is IComment comment)
        {
            var text = comment.Data.Trim();
            if (text.StartsWith(ExpressionPlaceholderPrefix) && expressions.TryGetValue(text, out var expr))
            {
                return expr;
            }
            // Skip stripped server comment placeholders
            if (text.StartsWith(CommentPlaceholderPrefix))
            {
                return null;
            }
            return new HtmlNode { Content = $"<!--{comment.Data}-->" };
        }

        if (domNode is IText textNode)
        {
            var text = textNode.Data;
            if (string.IsNullOrWhiteSpace(text))
                return null;
            return new HtmlNode { Content = text };
        }

        if (domNode is IElement element)
        {
            return ConvertDomElement(element, expressions, caseMapping);
        }

        return null;
    }

    private static AspxNode ConvertDomElement(IElement element, Dictionary<string, ExpressionNode> expressions, Dictionary<string, string> caseMapping)
    {
        var name = element.LocalName;

        // Elements that started as asp: are now asp_ prefixed
        if (name.StartsWith(AspPrefix, StringComparison.OrdinalIgnoreCase))
        {
            var tagName = name[AspPrefix.Length..];
            // Restore original case if we have it
            if (caseMapping.TryGetValue(tagName, out var originalCase))
            {
                tagName = originalCase;
            }
            return ConvertAspControl(element, tagName, expressions, caseMapping);
        }

        return ConvertHtmlElement(element, expressions, caseMapping);
    }

    private static AspControlNode ConvertAspControl(IElement element, string tagName, Dictionary<string, ExpressionNode> expressions, Dictionary<string, string> caseMapping)
    {
        var node = new AspControlNode { TagName = tagName };

        foreach (var attr in element.Attributes)
        {
            // Strip runat="server" attribute
            if (attr.LocalName.Equals("runat", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }
            node.Attributes[attr.LocalName] = attr.Value;
        }

        foreach (var child in element.ChildNodes)
        {
            var childNode = ConvertDomNode(child, expressions, caseMapping);
            if (childNode is not null)
            {
                node.Children.Add(childNode);
            }
        }

        return node;
    }

    private static AspxNode ConvertHtmlElement(IElement element, Dictionary<string, ExpressionNode> expressions, Dictionary<string, string> caseMapping)
    {
        // Check if this element contains asp_ children that need decomposition
        var hasAspChildren = HasAspDescendants(element);
        var hasExprPlaceholders = HasExpressionPlaceholders(element);

        if (!hasAspChildren && !hasExprPlaceholders)
        {
            // Pure HTML — output as-is
            return new HtmlNode { Content = element.OuterHtml };
        }

        // Has asp: children — decompose into opening tag, children, closing tag
        var children = new List<AspxNode>();

        var openTag = $"<{element.LocalName}";
        foreach (var attr in element.Attributes)
        {
            openTag += $" {attr.LocalName}=\"{attr.Value}\"";
        }
        openTag += ">";
        children.Add(new HtmlNode { Content = openTag });

        foreach (var child in element.ChildNodes)
        {
            var childNode = ConvertDomNode(child, expressions, caseMapping);
            if (childNode is not null)
            {
                children.Add(childNode);
            }
        }

        children.Add(new HtmlNode { Content = $"</{element.LocalName}>" });

        // Wrap mixed children in a virtual container
        var wrapper = new AspControlNode
        {
            TagName = "__html_wrapper__",
            Children = children
        };
        return wrapper;
    }

    private static bool HasAspDescendants(IElement element)
    {
        foreach (var child in element.ChildNodes)
        {
            if (child is IElement childElement)
            {
                if (childElement.LocalName.StartsWith(AspPrefix, StringComparison.OrdinalIgnoreCase))
                    return true;
                if (HasAspDescendants(childElement))
                    return true;
            }
        }
        return false;
    }

    private static bool HasExpressionPlaceholders(IElement element)
    {
        foreach (var child in element.ChildNodes)
        {
            if (child is IComment comment && comment.Data.Trim().StartsWith(ExpressionPlaceholderPrefix))
                return true;
            if (child is IElement childElement && HasExpressionPlaceholders(childElement))
                return true;
        }
        return false;
    }

    [GeneratedRegex(@"<%@\s*(\w+)\s+(.*?)%>", RegexOptions.Singleline)]
    private static partial Regex DirectiveRegex();

    [GeneratedRegex(@"(\w+)\s*=\s*[""']([^""']*)[""']")]
    private static partial Regex AttributeRegex();

    [GeneratedRegex(@"<%([#$=])?\s*(.*?)%>", RegexOptions.Singleline)]
    private static partial Regex ExpressionBlockRegex();

    [GeneratedRegex(@"<asp:", RegexOptions.IgnoreCase)]
    private static partial Regex AspOpenTagRegex();

    [GeneratedRegex(@"</asp:", RegexOptions.IgnoreCase)]
    private static partial Regex AspCloseTagRegex();

    [GeneratedRegex(@"<%--.*?--%>", RegexOptions.Singleline)]
    private static partial Regex ServerCommentRegex();

    [GeneratedRegex(@"</?asp:(\w+)", RegexOptions.None)]
    private static partial Regex AspTagRegex();

    [GeneratedRegex(@"<(asp_\w+)([^>]*?)\s*/>", RegexOptions.None)]
    private static partial Regex SelfClosingAspTagRegex();
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
