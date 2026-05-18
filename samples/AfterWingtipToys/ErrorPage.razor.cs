namespace WingtipToys;

public partial class ErrorPage
{
    protected override Task OnInitializedAsync()
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