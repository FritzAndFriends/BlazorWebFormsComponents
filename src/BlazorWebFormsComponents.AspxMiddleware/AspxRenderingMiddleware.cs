using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BlazorWebFormsComponents.AspxMiddleware;

/// <summary>
/// ASP.NET Core middleware that intercepts .aspx requests, parses the ASPX markup,
/// builds a Blazor component tree from BWFC components, and renders via HtmlRenderer.
/// </summary>
public class AspxRenderingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _aspxRootPath;
    private readonly ILogger<AspxRenderingMiddleware> _logger;

    public AspxRenderingMiddleware(RequestDelegate next, string aspxRootPath, ILogger<AspxRenderingMiddleware> logger)
    {
        _next = next;
        _aspxRootPath = aspxRootPath;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, HtmlRenderer htmlRenderer)
    {
        var path = context.Request.Path.Value;

        if (path is null || !path.EndsWith(".aspx", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        // Map URL path to file path
        var relativePath = path.TrimStart('/');
        var filePath = Path.Combine(_aspxRootPath, relativePath);

        if (!File.Exists(filePath))
        {
            _logger.LogWarning("ASPX file not found: {FilePath}", filePath);
            await _next(context);
            return;
        }

        try
        {
            var aspxContent = await File.ReadAllTextAsync(filePath);
            var parseResult = AspxParser.Parse(aspxContent);
            var fragment = AspxComponentTreeBuilder.Build(parseResult.Nodes);

            var title = parseResult.PageTitle ?? "ASPX Page";

            var html = await htmlRenderer.Dispatcher.InvokeAsync(async () =>
            {
                var parameters = ParameterView.FromDictionary(
                    new Dictionary<string, object?>
                    {
                        { nameof(AspxPageHost.Title), title },
                        { nameof(AspxPageHost.Body), fragment }
                    });

                var output = await htmlRenderer.RenderComponentAsync<AspxPageHost>(parameters);
                return output.ToHtmlString();
            });

            context.Response.ContentType = "text/html; charset=utf-8";
            await context.Response.WriteAsync(html);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rendering ASPX page: {Path}", path);
            context.Response.StatusCode = 500;
            await context.Response.WriteAsync($"<!-- Error rendering {path}: {ex.Message} -->");
        }
    }
}
