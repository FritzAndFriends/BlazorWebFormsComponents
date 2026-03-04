using Microsoft.AspNetCore.Components;

namespace WingtipToys
{
    public partial class ErrorPage : ComponentBase
    {
        [SupplyParameterFromQuery]
        public string? error { get; set; }

        [SupplyParameterFromQuery]
        public string? handler { get; set; }

        [SupplyParameterFromQuery]
        public string? detail { get; set; }

        [SupplyParameterFromQuery]
        public string? inner { get; set; }

        public string FriendlyMessage => error ?? "An error occurred while processing your request.";
        public bool ShowDetail => !string.IsNullOrEmpty(detail) || !string.IsNullOrEmpty(handler) || !string.IsNullOrEmpty(inner);
        public string DetailMessage => detail ?? "";
        public string HandlerMessage => handler ?? "";
        public string InnerMessage => inner ?? "";
    }
}
