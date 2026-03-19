using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace BlazorWebFormsComponents.AspxMiddleware;

/// <summary>
/// Host component that wraps rendered ASPX content in an HTML document shell.
/// Used by HtmlRenderer to produce a complete HTML page.
/// </summary>
public class AspxPageHost : ComponentBase
{
    [Parameter] public string Title { get; set; } = "ASPX Page";
    [Parameter] public RenderFragment? Body { get; set; }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.AddMarkupContent(0,
            $"""
            <!DOCTYPE html>
            <html>
            <head>
                <meta charset="utf-8" />
                <title>{System.Net.WebUtility.HtmlEncode(Title)}</title>
            </head>
            <body>
            """);

        if (Body is not null)
        {
            builder.AddContent(1, Body);
        }

        builder.AddMarkupContent(2,
            """

            </body>
            </html>
            """);
    }
}
