// =============================================================================
// TODO(bwfc-general): This code-behind was copied from Web Forms and needs manual migration.
//
// Common transforms needed (use the BWFC Copilot skill for assistance):
//   TODO(bwfc-lifecycle): Page_Load / Page_Init → OnInitializedAsync / OnParametersSetAsync
//   TODO(bwfc-lifecycle): Page_PreRender → OnAfterRenderAsync
//   TODO(bwfc-ispostback): IsPostBack checks → remove or convert to state logic
//   TODO(bwfc-viewstate): ViewState usage → component [Parameter] or private fields
//   TODO(bwfc-session-state): Session/Cache access → auto-wired on WebFormsPageBase via SessionShim/CacheShim
//   TODO(bwfc-navigation): Response.Redirect → auto-wired on WebFormsPageBase via ResponseShim
//   TODO(bwfc-form): Request.Form["key"] → auto-wired on WebFormsPageBase via FormShim (use <WebFormsForm> for interactive mode)
//   TODO(bwfc-server): Server.MapPath/HtmlEncode → auto-wired on WebFormsPageBase via ServerShim
//   TODO(bwfc-config): ConfigurationManager.AppSettings → BWFC shim (call app.UseConfigurationManagerShim() in Program.cs)
//   TODO(bwfc-general): ClientScript.RegisterStartupScript → auto-wired on WebFormsPageBase via ClientScriptShim
//   TODO(bwfc-general): Event handlers (Button_Click, etc.) → convert to Blazor event callbacks
//   TODO(bwfc-datasource): Data binding (DataBind, DataSource) → component parameters or OnInitialized
//   TODO(bwfc-general): ScriptManager code-behind references → use ScriptManagerShim via ScriptManager.GetCurrent(this)
//   TODO(bwfc-general): UpdatePanel markup preserved by BWFC (ContentTemplate supported) — remove only code-behind API calls
//   TODO(bwfc-general): User controls → Blazor component references
// =============================================================================


// --- ViewState Migration ---
// ViewState is in-memory only in Blazor (does not survive navigation).
// Convert to private fields or [Parameter] properties:
//   private object _sortColumn; // was ViewState["SortColumn"]
//   private object _sortDirection; // was ViewState["SortDirection"]
// Note: BaseWebFormsComponent.ViewState exists as an [Obsolete] compatibility shim.

// --- Session State Migration ---
// TODO(bwfc-session-state): Session["key"] calls work automatically via SessionShim on WebFormsPageBase.
// Session keys found: LastVisitDate
// Options for long-term replacement:
//   (1) ProtectedSessionStorage (Blazor Server) — persists across circuits
//   (2) Scoped service via DI — lifetime matches user circuit
//   (3) Cascading parameter from a root-level state provider
// See: https://learn.microsoft.com/aspnet/core/blazor/state-management

using System;
// using System.Configuration; // BWFC provides ConfigurationManager shim
namespace ContosoAdmin
{
    public partial class TC31_ComplexCodeBehind
    {
    // TODO(bwfc-general): ClientScript calls preserved — works via WebFormsPageBase (no injection needed). ScriptManagerShim may need @inject ScriptManagerShim ScriptManager for non-page classes.

    // --- Request.Form Migration ---
    // TODO(bwfc-form): Request.Form calls work automatically via RequestShim on WebFormsPageBase.
    // For interactive mode, wrap your form in <WebFormsForm OnSubmit="SetRequestFormData">.
    // Form keys found: key
    // For non-page classes, inject RequestShim via DI.

    // --- Response.Redirect Migration ---
    // TODO(bwfc-navigation): Response.Redirect() works via ResponseShim on WebFormsPageBase. Handles ~/ and .aspx automatically.
    // For non-page classes, inject ResponseShim via DI.

    // --- ConfigurationManager Migration ---
    // TODO(bwfc-config): ConfigurationManager calls work via BWFC shim.
    // Ensure app.UseConfigurationManagerShim() is called in Program.cs.
    // AppSettings keys found: AdminConnectionString
    // Add these to appsettings.json under "AppSettings" section or as top-level keys.

        protected override async Task OnInitializedAsync()
        {
            // TODO(bwfc-lifecycle): Review lifecycle conversion — verify async behavior
            await base.OnInitializedAsync();

                        // BWFC: IsPostBack guard unwrapped — Blazor re-renders on every state change
            LoadUsers();
                        lblStatus.Text = "Dashboard loaded.";

            var lastVisit = Session["LastVisitDate"];
            Session["LastVisitDate"] = DateTime.Now;

            ViewState["SortColumn"] = "UserName";
            var sortDir = ViewState["SortDirection"];
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // TODO(bwfc-lifecycle): Review lifecycle conversion — verify async behavior
            if (firstRender)
            {
                UpdateStatusBar();
            }
        }

        protected void Export_Click()
        {
            var reportPath = GetRouteUrl( /* TODO(bwfc-route-url): converted from Page route lookup – ensure page inherits WebFormsPageBase */ "ReportRoute", new { format = "csv" });
            Response.Redirect(reportPath);
        }

        protected void Refresh_Click()
        {
            Response.Redirect("/Admin/Dashboard");
        }

        protected void Users_RowCommand(GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                Response.Redirect("/Admin/UserDetail?id=" + e.CommandArgument);
            }
        }

        private void LoadUsers()
        {
            var connString = ConfigurationManager.AppSettings["AdminConnectionString"];
            // Load users from database
        }

        private void UpdateStatusBar()
        {
            // Update UI elements
        }


        private void HandlePostBack()
        {
            // TODO(bwfc-ispostback): Wire HandlePostBack() to appropriate Blazor event handlers (e.g., button Click, form Submit)
            lblStatus.Text = "Processing request...";
        }
    }
}
