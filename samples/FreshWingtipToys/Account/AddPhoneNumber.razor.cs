using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace WingtipToys.Account
{
    public partial class AddPhoneNumber : ComponentBase
    {
        private string phoneNumber = "";
        private string statusMessage = "";

        private Task HandleSubmit(MouseEventArgs args)
        {
            // Demo: phone number management would require SMS provider integration
            statusMessage = "Phone number functionality is not configured for this demo.";
            return Task.CompletedTask;
        }
    }
}
