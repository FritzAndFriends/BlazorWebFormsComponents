using Bunit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BlazorWebFormsComponents.Test;

/// <summary>
/// Base test context for BlazorWebFormsComponents tests.
/// Configures JSInterop and required services for component rendering.
/// </summary>
public abstract class BlazorWebFormsTestContext : BunitContext
{
    protected BlazorWebFormsTestContext()
    {
        // Configure bUnit's JSInterop to handle the OnAfterRender call
        // This is required because the base component calls this on first render
        JSInterop.SetupVoid("bwfc.Page.OnAfterRender");

        // Register required services for BaseWebFormsComponent
        Services.AddSingleton<LinkGenerator>(new Mock<LinkGenerator>().Object);
        Services.AddSingleton<IHttpContextAccessor>(new Mock<IHttpContextAccessor>().Object);
    }
}
