using System;

namespace BlazorWebFormsComponents;

/// <summary>
/// Compatibility shim for Web Forms <c>[QueryString("paramName")]</c> model-binding attribute.
/// <para>
/// In Web Forms, <c>[QueryString]</c> decorated method parameters to bind them from the
/// URL query string. In Blazor, this concept maps to <c>[SupplyParameterFromQuery]</c>
/// on component <b>properties</b> (not method parameters).
/// </para>
/// <para>
/// This attribute exists so migrated code-behinds compile at L1 without rewriting method
/// signatures. Layer 2 should promote query-bound parameters to
/// <c>[Parameter, SupplyParameterFromQuery]</c> properties on the component class.
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = false)]
public sealed class QueryStringAttribute : Attribute
{
    public QueryStringAttribute() { }

    public QueryStringAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    /// The query string parameter name to bind from.
    /// </summary>
    public string? Name { get; }
}
