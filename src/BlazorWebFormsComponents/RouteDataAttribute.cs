using System;

namespace BlazorWebFormsComponents;

/// <summary>
/// Compatibility shim for Web Forms <c>[RouteData]</c> model-binding attribute.
/// <para>
/// In Web Forms, <c>[RouteData]</c> decorated method parameters to bind them from
/// route data. In Blazor, this maps to <c>[Parameter]</c> properties on routable
/// components with route template tokens (e.g., <c>@page "/product/{Id:int}"</c>).
/// </para>
/// <para>
/// This attribute exists so migrated code-behinds compile at L1 without rewriting
/// method signatures. Layer 2 should promote route-bound parameters to
/// <c>[Parameter]</c> properties on the component class.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
public sealed class RouteDataAttribute : Attribute
{
}
