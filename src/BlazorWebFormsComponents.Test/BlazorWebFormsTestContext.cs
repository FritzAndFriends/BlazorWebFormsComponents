using Bunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit.Abstractions;

namespace BlazorWebFormsComponents.Test;

/// <summary>
/// Base test context for BlazorWebFormsComponents tests.
/// Configures JSInterop and required services for component rendering.
/// </summary>
public abstract class BlazorWebFormsTestContext : BunitContext
{
    protected BlazorWebFormsTestContext() : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="BlazorWebFormsTestContext"/> class.
    /// </summary>
    /// <param name="outputHelper">Optional xUnit test output helper for logging. When provided, enables logging output in test results.</param>
    protected BlazorWebFormsTestContext(ITestOutputHelper outputHelper)
    {
        // Configure bUnit's JSInterop to handle the OnAfterRender call
        // This is required because the base component calls this on first render
        JSInterop.SetupVoid("bwfc.Page.OnAfterRender");

        // Register required services for BaseWebFormsComponent
        Services.AddSingleton<LinkGenerator>(new Mock<LinkGenerator>().Object);
        Services.AddSingleton<IHttpContextAccessor>(new Mock<IHttpContextAccessor>().Object);

        // Add xUnit logger if output helper is provided
        if (outputHelper != null)
        {
            Services.AddLogging(builder => builder.AddXUnit(outputHelper));
        }
    }
}
