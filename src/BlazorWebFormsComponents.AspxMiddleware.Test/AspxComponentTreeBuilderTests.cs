using BlazorWebFormsComponents.AspxMiddleware;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging.Abstractions;
using Shouldly;

namespace BlazorWebFormsComponents.AspxMiddleware.Test;

public class AspxComponentTreeBuilderTests
{
    [Fact]
    public void Build_HtmlNode_RendersRawHtml()
    {
        var nodes = new List<AspxNode>
        {
            new HtmlNode { Content = "<h1>Hello</h1>" }
        };

        var fragment = AspxComponentTreeBuilder.Build(nodes);
        fragment.ShouldNotBeNull();
    }

    [Fact]
    public void Build_UnrecognizedControl_RendersComment()
    {
        var nodes = new List<AspxNode>
        {
            new AspControlNode
            {
                TagName = "UnknownWidget",
                Attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["ID"] = "w1"
                }
            }
        };

        var fragment = AspxComponentTreeBuilder.Build(nodes);
        fragment.ShouldNotBeNull();
    }

    [Fact]
    public void Build_ExpressionNode_RendersComment()
    {
        var nodes = new List<AspxNode>
        {
            new ExpressionNode { ExpressionType = "eval", Expression = "DateTime.Now" }
        };

        var fragment = AspxComponentTreeBuilder.Build(nodes);
        fragment.ShouldNotBeNull();
    }

    [Fact]
    public void Build_KnownControl_ProducesRenderFragment()
    {
        var nodes = new List<AspxNode>
        {
            new AspControlNode
            {
                TagName = "Label",
                Attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                {
                    ["Text"] = "Hello World",
                    ["ID"] = "lbl1"
                }
            }
        };

        var fragment = AspxComponentTreeBuilder.Build(nodes);
        fragment.ShouldNotBeNull();
    }

    [Fact]
    public void Build_NestedControls_ProducesRenderFragment()
    {
        var nodes = new List<AspxNode>
        {
            new AspControlNode
            {
                TagName = "Panel",
                Attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) { ["ID"] = "pnl1" },
                Children =
                [
                    new AspControlNode
                    {
                        TagName = "Label",
                        Attributes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
                        {
                            ["Text"] = "Inside",
                            ["ID"] = "lbl1"
                        }
                    }
                ]
            }
        };

        var fragment = AspxComponentTreeBuilder.Build(nodes);
        fragment.ShouldNotBeNull();
    }
}
