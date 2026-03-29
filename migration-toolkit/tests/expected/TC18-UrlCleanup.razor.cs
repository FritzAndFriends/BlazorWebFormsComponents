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
using System;

namespace MyApp
{
    public partial class TC18_UrlCleanup
    {
    [Inject] private NavigationManager NavigationManager { get; set; } // TODO: Add @using Microsoft.AspNetCore.Components to _Imports.razor if needed

        protected void Page_Load(object sender, EventArgs e)
        {
            NavigationManager.NavigateTo("/Products.aspx");
            NavigationManager.NavigateTo("/Admin/Dashboard.aspx?id=1");
            string url = "~/Help.aspx#section";
            var path = "~/Images/logo.png";
        }

        protected void Button_Click(object sender, EventArgs e)
        {
            NavigationManager.NavigateTo("/Checkout.aspx");
        }
    }
}
