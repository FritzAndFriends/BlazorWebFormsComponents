using Microsoft.JSInterop;

namespace BlazorWebFormsComponents.AspxMiddleware.Test;

/// <summary>
/// No-op JSRuntime for SSR rendering tests where JS interop is not available.
/// </summary>
internal class NoOpJsRuntime : IJSRuntime
{
    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        => default;

    public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        => default;
}
