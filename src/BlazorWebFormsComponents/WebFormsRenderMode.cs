namespace BlazorWebFormsComponents;

/// <summary>
/// Identifies the current rendering mode of a Blazor Web Forms component.
/// Used internally by ViewState and IsPostBack to adapt behavior automatically.
/// </summary>
public enum WebFormsRenderMode
{
	/// <summary>
	/// Static Server-Side Rendering or pre-render — HttpContext is available, no SignalR circuit.
	/// ViewState persists via a protected hidden form field. IsPostBack checks the HTTP method.
	/// </summary>
	StaticSSR,

	/// <summary>
	/// Interactive Server rendering — SignalR circuit is active, no HttpContext.
	/// ViewState persists in component instance memory. IsPostBack tracks initialization state.
	/// </summary>
	InteractiveServer
}
