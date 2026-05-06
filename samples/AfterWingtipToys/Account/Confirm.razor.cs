namespace WingtipToys.Account
{
    public partial class Confirm : BlazorWebFormsComponents.WebFormsPageBase
    {
        private string Title { get; set; } = "Confirm";
        private PlaceHolder successPanel = default!;
        private HyperLink login = default!;
        private PlaceHolder errorPanel = default!;
    }
}
