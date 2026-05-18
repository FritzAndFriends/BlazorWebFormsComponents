using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace MyApp
{
    public partial class TC29_Combined : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                Session["LastVisit"] = DateTime.Now;
                BindGrid();
            }
        }

        protected void SaveButton_Click(object sender, EventArgs e)
        {
            // Save logic
            Response.Redirect("~/Confirmation.aspx");
        }

        private void BindGrid()
        {
            // Load data
        }
    }
}
