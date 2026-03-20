using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlazorWebFormsComponents.AspxMiddleware;

/// <summary>
/// Walks a parsed ASPX AST and builds a Blazor RenderFragment
/// using RenderTreeBuilder to compose BWFC components.
/// </summary>
public static class AspxComponentTreeBuilder
{
    /// <summary>
    /// Build a RenderFragment from parsed ASPX nodes.
    /// </summary>
    public static RenderFragment Build(IEnumerable<AspxNode> nodes)
    {
        return builder =>
        {
            var seq = 0;
            foreach (var node in nodes)
            {
                RenderNode(builder, node, ref seq);
            }
        };
    }

    private static void RenderNode(RenderTreeBuilder builder, AspxNode node, ref int seq)
    {
        switch (node)
        {
            case AspControlNode control:
                RenderAspControl(builder, control, ref seq);
                break;

            case HtmlNode html:
                builder.AddMarkupContent(seq++, html.Content);
                break;

            case ExpressionNode expr:
                // Render expressions as HTML comments (Phase 1 — no expression evaluation)
                var comment = $"<!-- ASPX Expression ({expr.ExpressionType}): {System.Net.WebUtility.HtmlEncode(expr.Expression)} -->";
                builder.AddMarkupContent(seq++, comment);
                break;

            case DirectiveNode:
                // Directives are metadata, not rendered
                break;
        }
    }

    private static void RenderAspControl(RenderTreeBuilder builder, AspControlNode control, ref int seq)
    {
        // Special case: virtual HTML wrapper (mixed HTML + asp: children)
        if (control.TagName == "__html_wrapper__")
        {
            foreach (var child in control.Children)
            {
                RenderNode(builder, child, ref seq);
            }
            return;
        }

        var componentType = AspxComponentRegistry.Resolve(control.TagName);

        if (componentType is null)
        {
            // Unrecognized asp: tag — render as HTML comment
            builder.AddMarkupContent(seq++, $"<!-- Unsupported: asp:{control.TagName} -->");

            // Still render children in case they contain recognized controls
            foreach (var child in control.Children)
            {
                RenderNode(builder, child, ref seq);
            }
            return;
        }

        builder.OpenComponent(seq++, componentType);

        // Map attributes to component parameters
        foreach (var attr in control.Attributes)
        {
            var value = CoerceAttributeValue(attr.Key, attr.Value, componentType);
            builder.AddAttribute(seq++, attr.Key, value);
        }

        // Add child content if present
        if (control.Children.Count > 0)
        {
            builder.AddAttribute(seq++, "ChildContent", (RenderFragment)(childBuilder =>
            {
                var childSeq = 0;
                foreach (var child in control.Children)
                {
                    RenderNode(childBuilder, child, ref childSeq);
                }
            }));
        }

        builder.CloseComponent();
    }

    /// <summary>
    /// Basic type coercion for common attribute types.
    /// ASPX attributes are all strings; BWFC components may expect typed parameters.
    /// </summary>
    private static object CoerceAttributeValue(string name, string value, Type componentType)
    {
        // Try to find the property on the component type for type inference
        var property = componentType.GetProperty(name,
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance |
            System.Reflection.BindingFlags.IgnoreCase);

        if (property is null)
        {
            return value; // Unknown property — pass as string
        }

        var targetType = property.PropertyType;

        // Handle nullable types
        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        // Boolean
        if (underlyingType == typeof(bool))
        {
            return bool.TryParse(value, out var b) ? b : value;
        }

        // Int32
        if (underlyingType == typeof(int))
        {
            return int.TryParse(value, out var i) ? i : value;
        }

        // Double
        if (underlyingType == typeof(double))
        {
            return double.TryParse(value, out var d) ? d : value;
        }

        // Unit (BWFC's web forms Unit type)
        if (underlyingType == typeof(Unit))
        {
            return Unit.Parse(value);
        }

        // Enums
        if (underlyingType.IsEnum)
        {
            return Enum.TryParse(underlyingType, value, ignoreCase: true, out var e) ? e! : value;
        }

        // EventCallback (non-generic) — Phase 1: no-op callback, handler name preserved as comment
        if (targetType == typeof(EventCallback))
        {
            return EventCallback.Empty;
        }

        // EventCallback<T> — Phase 1: default (no-op) typed callback
        if (targetType.IsGenericType && targetType.GetGenericTypeDefinition() == typeof(EventCallback<>))
        {
            return Activator.CreateInstance(targetType)!;
        }

        // Default: return as string
        return value;
    }
}
