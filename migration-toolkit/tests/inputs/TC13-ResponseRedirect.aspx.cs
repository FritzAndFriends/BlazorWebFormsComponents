using System;

namespace MyApp
{
    public partial class TC13_ResponseRedirect : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Redirect("~/Products.aspx");
            Response.Redirect("/Cart.aspx", false);
            Response.Redirect(GetUrl());
        }
    }
}
