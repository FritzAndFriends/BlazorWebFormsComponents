using System;

namespace MyApp
{
    public partial class TC26_GetRouteUrl : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var url = Page.GetRouteUrl("ProductRoute", new { id = 42 });
            ProductLink.NavigateUrl = url;
        }
    }
}
