using System;

namespace MyApp
{
    public partial class TC18_UrlCleanup : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("~/Products.aspx");
            Response.Redirect("~/Admin/Dashboard.aspx?id=1");
            string url = "~/Help.aspx#section";
            var path = "~/Images/logo.png";
        }

        protected void Button_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Checkout.aspx", false);
        }
    }
}
