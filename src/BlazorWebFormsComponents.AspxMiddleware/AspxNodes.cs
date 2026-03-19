namespace BlazorWebFormsComponents.AspxMiddleware;

/// <summary>
/// Base class for all nodes in the parsed ASPX AST.
/// </summary>
public abstract class AspxNode
{
}

/// <summary>
/// Represents a page directive like &lt;%@ Page Title="Home" MasterPageFile="~/Site.Master" %&gt;
/// </summary>
public class DirectiveNode : AspxNode
{
    public string DirectiveType { get; set; } = string.Empty;
    public Dictionary<string, string> Attributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);
}

/// <summary>
/// Represents an asp: server control like &lt;asp:Button Text="Submit" /&gt;
/// </summary>
public class AspControlNode : AspxNode
{
    public string TagName { get; set; } = string.Empty;
    public Dictionary<string, string> Attributes { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public List<AspxNode> Children { get; set; } = [];
}

/// <summary>
/// Represents raw HTML content (non-asp: elements and text).
/// </summary>
public class HtmlNode : AspxNode
{
    public string Content { get; set; } = string.Empty;
}

/// <summary>
/// Placeholder for server-side expressions (&lt;% %&gt;, &lt;%# %&gt;, &lt;%$ %&gt;, &lt;%= %&gt;).
/// </summary>
public class ExpressionNode : AspxNode
{
    public string ExpressionType { get; set; } = string.Empty; // "code", "databind", "resource", "eval"
    public string Expression { get; set; } = string.Empty;
}
