using System;
using System.Web.UI.WebControls;

namespace MyApp
{
    public partial class TC21_EventHandlerSpecialized : System.Web.UI.Page
    {
        protected void Grid_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Handle row command
        }

        protected void Grid_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            // Handle paging
        }
    }
}
