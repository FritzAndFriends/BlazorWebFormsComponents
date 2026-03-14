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
    /// <summary>
    /// Initializes a new instance of the <see cref="BlazorWebFormsTestContext"/> class without logging support.
    /// </summary>
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

        // Always register logging so ILogger<T> is resolvable.
        // Add xUnit output sink when a test output helper is provided.
        Services.AddLogging(builder =>
        {
            if (outputHelper != null)
                builder.AddXUnit(outputHelper);
        });
    }
}
