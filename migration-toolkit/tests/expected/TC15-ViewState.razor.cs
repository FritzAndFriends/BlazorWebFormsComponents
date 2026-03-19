// =============================================================================
// TODO: This code-behind was copied from Web Forms and needs manual migration.
//
// Common transforms needed (use the BWFC Copilot skill for assistance):
//   - Page_Load / Page_Init → OnInitializedAsync / OnParametersSetAsync
//   - Page_PreRender → OnAfterRenderAsync
//   - IsPostBack checks → remove or convert to state logic
//   - ViewState usage → component [Parameter] or private fields
//   - Session/Cache access → inject IHttpContextAccessor or use DI
//   - Response.Redirect → NavigationManager.NavigateTo
//   - Event handlers (Button_Click, etc.) → convert to Blazor event callbacks
//   - Data binding (DataBind, DataSource) → component parameters or OnInitialized
//   - ScriptManager code-behind references → remove (Blazor handles updates)
//   - UpdatePanel markup preserved by BWFC (ContentTemplate supported) — remove only code-behind API calls
//   - User controls → Blazor component references
// =============================================================================

// --- ViewState Migration ---
// ViewState is in-memory only in Blazor (does not survive navigation).
// Convert to private fields or [Parameter] properties:
//   private object _sortOrder; // was ViewState["SortOrder"]
//   private object _filterText; // was ViewState["FilterText"]
// Note: BaseWebFormsComponent.ViewState exists as an [Obsolete] compatibility shim.

using System;

namespace MyApp
{
    public partial class TC15_ViewState : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ViewState["SortOrder"] = "ASC";
            var filter = ViewState["FilterText"];
        }
    }
}

