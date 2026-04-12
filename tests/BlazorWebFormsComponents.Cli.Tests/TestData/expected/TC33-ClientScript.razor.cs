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
using System;

namespace MyApp
{
    public partial class TC33_ClientScript
    {
    // TODO(bwfc-general): ClientScript calls preserved — uses ClientScriptShim + ScriptManagerShim. Inject @inject ClientScriptShim ClientScript and @inject ScriptManagerShim ScriptManager if not using BaseWebFormsComponent.

    // --- Request.Form Migration ---
    // TODO(bwfc-form): Request.Form calls work via FormShim on WebFormsPageBase.
    // For interactive mode, wrap your form in <WebFormsForm OnSubmit="SetRequestFormData">.
    // Form keys found: key
    // For non-page classes, inject RequestShim via DI.

    // --- ConfigurationManager Migration ---
    // TODO(bwfc-config): ConfigurationManager calls work via BWFC shim.
    // Ensure app.UseConfigurationManagerShim() is called in Program.cs.

        protected override async Task OnInitializedAsync()
        {
            // TODO(bwfc-lifecycle): Review lifecycle conversion — verify async behavior
            await base.OnInitializedAsync();

            // Pattern 1: RegisterStartupScript with inline script
            ClientScript.RegisterStartupScript(
                this.GetType(),
                "InitializeUI",
                "$(function() { console.log('ready'); });",
                true);

            // Pattern 2: RegisterClientScriptInclude with URL
            ClientScript.RegisterClientScriptInclude(
                "jqueryUI",
                ResolveUrl("~/Scripts/jquery-ui.min.js"));

            // Pattern 3: GetPostBackEventReference
            var postbackRef = ClientScript.GetPostBackEventReference(btnSubmit, "validate");

            // Pattern 4: RegisterClientScriptBlock
            ClientScript.RegisterClientScriptBlock(this.GetType(), "block1", "<script>var x = 1;</script>");

            // Pattern 5: ScriptManager.RegisterStartupScript
            ClientScript.RegisterStartupScript(this.GetType(), "smScript", "alert('hello');", true);

            // Pattern 6: ScriptManager.GetCurrent
            var sm = ScriptManager.GetCurrent(this);
        }
    }
}
