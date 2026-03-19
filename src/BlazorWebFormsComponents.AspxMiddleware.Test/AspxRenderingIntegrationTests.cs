using BlazorWebFormsComponents.AspxMiddleware;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;

namespace BlazorWebFormsComponents.AspxMiddleware.Test;

/// <summary>
/// Integration test: full pipeline (parse → build → render) for ASPX files.
/// </summary>
public class AspxRenderingIntegrationTests
{
    [Fact]
    public async Task RenderSimpleAspx_ProducesHtmlOutput()
    {
        var content = File.ReadAllText(GetTestPagePath("Simple.aspx"));
        var parseResult = AspxParser.Parse(content);
        var fragment = AspxComponentTreeBuilder.Build(parseResult.Nodes);

        var html = await RenderFragmentToHtml(fragment, parseResult.PageTitle ?? "Test");

        html.ShouldNotBeNullOrWhiteSpace();
        html.ShouldContain("<!DOCTYPE html>");
        html.ShouldContain("<title>Simple Test</title>");
        // The rendered output should contain button and label HTML
        html.ShouldContain("Submit");
        html.ShouldContain("Hello World");
    }

    [Fact]
    public async Task RenderNestedAspx_ProducesHtmlOutput()
    {
        var content = File.ReadAllText(GetTestPagePath("Nested.aspx"));
        var parseResult = AspxParser.Parse(content);
        var fragment = AspxComponentTreeBuilder.Build(parseResult.Nodes);

        var html = await RenderFragmentToHtml(fragment, parseResult.PageTitle ?? "Test");

        html.ShouldNotBeNullOrWhiteSpace();
        html.ShouldContain("Inside Panel");
    }

    [Fact]
    public async Task RenderMixedAspx_ProducesHtmlWithStaticContent()
    {
        var content = File.ReadAllText(GetTestPagePath("Mixed.aspx"));
        var parseResult = AspxParser.Parse(content);
        var fragment = AspxComponentTreeBuilder.Build(parseResult.Nodes);

        var html = await RenderFragmentToHtml(fragment, parseResult.PageTitle ?? "Test");

        html.ShouldNotBeNullOrWhiteSpace();
        html.ShouldContain("Welcome");
        html.ShouldContain("static HTML");
        // Should contain an expression comment marker
        html.ShouldContain("ASPX Expression");
    }

    private static async Task<string> RenderFragmentToHtml(RenderFragment fragment, string title)
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddBlazorWebFormsComponents();
        services.AddRouting();
        // BWFC components inject IJSRuntime — provide a no-op implementation for SSR
        services.AddSingleton<Microsoft.JSInterop.IJSRuntime, NoOpJsRuntime>();
        var sp = services.BuildServiceProvider();

        await using var htmlRenderer = new HtmlRenderer(sp, sp.GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>());

        return await htmlRenderer.Dispatcher.InvokeAsync(async () =>
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
    }

    private static string GetTestPagePath(string fileName)
    {
        return Path.Combine(AppContext.BaseDirectory, "TestPages", fileName);
    }
}
