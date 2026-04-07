using Bunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.IO;
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
        // Configure bUnit's JSInterop to handle all JS calls from lifecycle methods
        JSInterop.Mode = JSRuntimeMode.Loose;

        // Register required services for BaseWebFormsComponent and WebFormsPageBase
        Services.AddSingleton<LinkGenerator>(new Mock<LinkGenerator>().Object);
        Services.AddSingleton<IHttpContextAccessor>(new Mock<IHttpContextAccessor>().Object);
        Services.AddSingleton<IWebHostEnvironment>(CreateMockWebHostEnvironment());
        Services.AddMemoryCache();
        Services.AddScoped<CacheShim>();
        Services.AddScoped<ServerShim>();
        Services.AddScoped<ClientScriptShim>();

        // Always register logging so ILogger<T> is resolvable.
        // Add xUnit output sink when a test output helper is provided.
        Services.AddLogging(builder =>
        {
            if (outputHelper != null)
                builder.AddXUnit(outputHelper);
        });
    }

    private static IWebHostEnvironment CreateMockWebHostEnvironment()
    {
        var mock = new Mock<IWebHostEnvironment>();
        mock.Setup(e => e.WebRootPath).Returns(Path.Combine(Path.GetTempPath(), "wwwroot"));
        mock.Setup(e => e.ContentRootPath).Returns(Path.GetTempPath());
        return mock.Object;
    }
}
