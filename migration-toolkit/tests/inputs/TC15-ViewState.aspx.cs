using System;

namespace MyApp
{
    public partial class TC15_ViewState : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ViewState["SortOrder"] = "ASC";
            var filter = ViewState["FilterText"];
        }
    }
}
