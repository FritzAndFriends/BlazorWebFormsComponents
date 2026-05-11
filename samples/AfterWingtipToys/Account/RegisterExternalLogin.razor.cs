using BlazorWebFormsComponents;
using BlazorWebFormsComponents.Enums;

namespace WingtipToys.Account;

public partial class RegisterExternalLogin
{
    protected string ProviderName { get; set; } = string.Empty;
    protected ValidatorDisplay Dynamic { get; } = ValidatorDisplay.Dynamic;
    private TextBox email = default!;

    protected override void OnInitialized()
    {
        base.OnInitialized();
        ProviderName = Request.QueryString["provider"].ToString();
    }

    private void LogIn_Click(EventArgs args)
    {
    }
}

