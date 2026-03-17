using System;

namespace MyApp
{
    public partial class TC14_SessionDetect : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var cartId = Session["CartId"];
            Session["UserName"] = "test";
        }
    }
}
