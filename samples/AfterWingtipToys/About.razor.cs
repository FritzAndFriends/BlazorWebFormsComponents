using Microsoft.AspNetCore.Components;

namespace WingtipToys
{
  public partial class About
  {
    protected override async Task OnInitializedAsync()
    {
      Page.Title = "About";
    }
  }
}
