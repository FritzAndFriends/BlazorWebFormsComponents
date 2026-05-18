using BlazorWebFormsComponents;
using System.Data;

namespace AfterDepartmentPortal.Components.Pages;

/// <summary>
/// Demo page that triggers BWFC Roslyn analyzers for documentation screenshots.
/// Open this file in Visual Studio to see analyzer squiggles.
/// This file is intentionally NOT meant to be "clean" — it exists to demonstrate the analyzers.
/// </summary>
public partial class AnalyzerDemo
{
    // -------------------------------------------------------
    // BWFC002: ViewState usage (ℹ️ Info)
    // Hover the green squiggle on ViewState["..."] to see the tooltip.
    // -------------------------------------------------------
    private void LoadSortOrder()
    {
        ViewState["SortDirection"] = "ASC";
        var direction = (string)ViewState["SortDirection"];
    }

    // -------------------------------------------------------
    // BWFC003: IsPostBack usage (ℹ️ Info)
    // Hover the green squiggle on IsPostBack to see the tooltip.
    // -------------------------------------------------------
    protected override void OnInitialized()
    {
        if (!IsPostBack)
        {
            LoadSortOrder();
        }
    }

    // -------------------------------------------------------
    // BWFC025: Non-serializable ViewState type (⚠️ Warning)
    // Hover the yellow squiggle on the assignment to see the tooltip.
    // -------------------------------------------------------
    private void LoadData()
    {
        var table = new DataTable();
        ViewState["Results"] = table;
    }
}
