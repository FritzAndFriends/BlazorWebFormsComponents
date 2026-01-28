using System;
using System.Threading.Tasks;
using Microsoft.JSInterop;

namespace BlazorWebFormsComponents;

/// <summary>
/// Service that manages JavaScript interop for BlazorWebFormsComponents.
/// Automatically loads the required JavaScript module on first use.
/// </summary>
public sealed class BlazorWebFormsJsInterop : IAsyncDisposable
{
    private readonly Lazy<Task<IJSObjectReference>> _moduleTask;
    private readonly IJSRuntime _jsRuntime;
    private bool _globalScriptLoaded;

    public BlazorWebFormsJsInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
        _moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
            "import", "./_content/Fritz.BlazorWebFormsComponents/js/Basepage.module.js").AsTask());
    }

    /// <summary>
    /// Ensures the global script is loaded (for backward compatibility)
    /// </summary>
    internal async ValueTask EnsureScriptLoadedAsync()
    {
        if (_globalScriptLoaded) return;

        try
        {
            // Import the module which will also set up window.bwfc
            await _moduleTask.Value;
            _globalScriptLoaded = true;
        }
        catch (JSException)
        {
            // Script may already be loaded via script tag - that's OK
            _globalScriptLoaded = true;
        }
    }

    /// <summary>
    /// Calls the OnAfterRender JavaScript function
    /// </summary>
    internal async ValueTask OnAfterRenderAsync()
    {
        await EnsureScriptLoadedAsync();

        try
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("onAfterRender");
        }
        catch (JSException)
        {
            // Fall back to global function if module approach fails
            await _jsRuntime.InvokeVoidAsync("bwfc.Page.OnAfterRender");
        }
    }

    /// <summary>
    /// Sets the page title
    /// </summary>
    public async ValueTask SetTitleAsync(string title)
    {
        await EnsureScriptLoadedAsync();

        try
        {
            var module = await _moduleTask.Value;
            await module.InvokeVoidAsync("setTitle", title);
        }
        catch (JSException)
        {
            await _jsRuntime.InvokeVoidAsync("bwfc.Page.setTitle", title);
        }
    }

    /// <summary>
    /// Gets the page title
    /// </summary>
    public async ValueTask<string> GetTitleAsync()
    {
        await EnsureScriptLoadedAsync();

        try
        {
            var module = await _moduleTask.Value;
            return await module.InvokeAsync<string>("getTitle");
        }
        catch (JSException)
        {
            return await _jsRuntime.InvokeAsync<string>("bwfc.Page.getTitle");
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_moduleTask.IsValueCreated)
        {
            var module = await _moduleTask.Value;
            await module.DisposeAsync();
        }
    }
}
