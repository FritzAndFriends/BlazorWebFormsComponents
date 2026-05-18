using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

namespace ContosoAdmin
{
    public partial class Dashboard : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadUsers();
                lblStatus.Text = "Dashboard loaded.";
            }
            else
            {
                lblStatus.Text = "Processing request...";
            }

            var lastVisit = Session["LastVisitDate"];
            Session["LastVisitDate"] = DateTime.Now;

            ViewState["SortColumn"] = "UserName";
            var sortDir = ViewState["SortDirection"];
        }

        protected void Page_PreRender(object sender, EventArgs e)
        {
            UpdateStatusBar();
        }

        protected void Export_Click(object sender, EventArgs e)
        {
            var reportPath = Page.GetRouteUrl("ReportRoute", new { format = "csv" });
            Response.Redirect(reportPath);
        }

        protected void Refresh_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/Admin/Dashboard.aspx");
        }

        protected void Users_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Select")
            {
                Response.Redirect("~/Admin/UserDetail.aspx?id=" + e.CommandArgument);
            }
        }

        private void LoadUsers()
        {
            var connString = ConfigurationManager.AppSettings["AdminConnectionString"];
            // Load users from database
        }

        private void UpdateStatusBar()
        {
            // Update UI elements
        }
    }
}
