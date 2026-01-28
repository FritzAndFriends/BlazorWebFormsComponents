using Bunit;

namespace BlazorWebFormsComponents.Test;

/// <summary>
/// Base test context for BlazorWebFormsComponents tests.
/// Configures JSInterop to handle the Page.OnAfterRender call.
/// </summary>
public abstract class BlazorWebFormsTestContext : BunitContext
{
    protected BlazorWebFormsTestContext()
    {
        // Configure bUnit's JSInterop to handle the OnAfterRender call
        // This is required because the base component calls this on first render
        JSInterop.SetupVoid("bwfc.Page.OnAfterRender");
    }
}
