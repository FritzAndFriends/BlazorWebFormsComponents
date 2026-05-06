// =============================================================================
// TODO(bwfc-general): This code-behind was copied from Web Forms and needs manual migration.
// =============================================================================
using System;
using System.Collections.Generic;
using System.Linq;

namespace WingtipToys
{
  public partial class Default
  {
    protected override async Task OnInitializedAsync()
    {
      await base.OnInitializedAsync();
    }

    private void Page_Error()
    {
      // Error handling is managed through ASP.NET Core middleware in the migrated app.
    }
  }
}
