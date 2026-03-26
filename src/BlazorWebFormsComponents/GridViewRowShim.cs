using System;

namespace BlazorWebFormsComponents;

/// <summary>
/// Non-generic compatibility shim for Web Forms <c>System.Web.UI.WebControls.GridViewRow</c>.
/// <para>
/// Web Forms code often passes <c>GridViewRow</c> without a type parameter (e.g., in
/// helper methods like <c>GetValues(GridViewRow row)</c>). The BWFC Blazor component is
/// generic (<c>GridViewRow&lt;T&gt;</c>), so this non-generic class bridges the gap at L1.
/// </para>
/// <para>
/// Layer 2 should refactor code to use the generic <c>GridViewRow&lt;T&gt;</c> component
/// or replace the Web Forms row-manipulation pattern entirely.
/// </para>
/// </summary>
[Obsolete("Use GridViewRow<T> for Blazor components. This non-generic shim exists for L1 migration compilation only.")]
public class GridViewRow
{
    public int RowIndex { get; set; }
    public int DataItemIndex { get; set; }
    public object DataItem { get; set; } = null!;
}
