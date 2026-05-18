// =============================================================================
// TODO(bwfc-general): This code-behind was copied from Web Forms and needs manual migration.
//
// Common transforms needed (use the BWFC Copilot skill for assistance):
//   TODO(bwfc-lifecycle): Page_Load / Page_Init → OnInitializedAsync / OnParametersSetAsync
//   TODO(bwfc-lifecycle): Page_PreRender → OnAfterRenderAsync
//   TODO(bwfc-ispostback): IsPostBack checks → remove or convert to state logic
//   TODO(bwfc-viewstate): ViewState usage → component [Parameter] or private fields
//   TODO(bwfc-session-state): Session/Cache access → inject IHttpContextAccessor or use DI
//   TODO(bwfc-navigation): Response.Redirect → NavigationManager.NavigateTo
//   TODO(bwfc-general): Event handlers (Button_Click, etc.) → convert to Blazor event callbacks
//   TODO(bwfc-datasource): Data binding (DataBind, DataSource) → component parameters or OnInitialized
//   TODO(bwfc-general): ScriptManager code-behind references → remove (Blazor handles updates)
//   TODO(bwfc-general): UpdatePanel markup preserved by BWFC (ContentTemplate supported) — remove only code-behind API calls
//   TODO(bwfc-general): User controls → Blazor component references
// =============================================================================
using System;

namespace MyApp
{
    public partial class TC18_UrlCleanup
    {
    [Inject] private NavigationManager NavigationManager { get; set; } // TODO(bwfc-navigation): Add @using Microsoft.AspNetCore.Components to _Imports.razor if needed

        protected override async Task OnInitializedAsync()
        {
            // TODO(bwfc-lifecycle): Review lifecycle conversion — verify async behavior
            await base.OnInitializedAsync();

            NavigationManager.NavigateTo("/Products.aspx");
            NavigationManager.NavigateTo("/Admin/Dashboard.aspx?id=1");
            string url = "~/Help.aspx#section";
            var path = "~/Images/logo.png";
        }

        protected void Button_Click()
        {
            NavigationManager.NavigateTo("/Checkout.aspx");
        }
    }
}
