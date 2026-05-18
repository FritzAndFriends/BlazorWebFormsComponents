using BlazorWebFormsComponents.Cli.Pipeline;

namespace BlazorWebFormsComponents.Cli.Transforms.CodeBehind;

/// <summary>
/// Injects the TODO migration guidance header at the top of code-behind files.
/// Must run first so other transforms can reference the header marker.
/// </summary>
public class TodoHeaderTransform : ICodeBehindTransform
{
    public string Name => "TodoHeader";
    public int Order => 10;

    private const string TodoHeader = """
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

        """;

    public string Apply(string content, FileMetadata metadata)
    {
        return TodoHeader + content;
    }
}
