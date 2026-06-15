namespace WingtipToys;

public partial class ErrorPage
{
  public partial class ErrorPage : WebFormsPageBase
  {
    // TODO(bwfc-general): ClientScript calls preserved — works via WebFormsPageBase (no injection needed). ScriptManagerShim may need @inject ScriptManagerShim ScriptManager for non-page classes.

    // --- Server Utility Migration ---
    // TODO(bwfc-server): Server.* calls work automatically via ServerShim on WebFormsPageBase.
    // Methods found: GetLastError, ClearError
    // For non-page classes, inject ServerShim via DI.
    // GetLastError() currently returns null and ClearError() is a no-op compatibility stub.

    // --- Request.Form Migration ---
    // TODO(bwfc-form): Request.Form calls work automatically via RequestShim on WebFormsPageBase.
    // For interactive mode, wrap your form in <WebFormsForm OnSubmit="SetRequestFormData">.
    // Form keys found: key
    // For non-page classes, inject RequestShim via DI.

    private Panel DetailedErrorPanel = default!;
    protected override async Task OnInitializedAsync()
    {
        Title = "Error";
        return base.OnInitializedAsync();
    }
  

    private object? _ErrorDetailedMsg_Text; // TODO: migrate from Web Forms code-behind

    private object? _ErrorHandler_Text; // TODO: migrate from Web Forms code-behind

    private object? _FriendlyErrorMsg_Text; // TODO: migrate from Web Forms code-behind

    private object? _InnerMessage_Text; // TODO: migrate from Web Forms code-behind

    private object? _InnerTrace_Text; // TODO: migrate from Web Forms code-behind
}
}