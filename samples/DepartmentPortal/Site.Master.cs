using System;
using System.Web.UI.WebControls;

namespace DepartmentPortal
{
    public partial class SiteMaster : BaseMasterPage
    {
        protected HyperLink LoginLink;
        protected LinkButton LogoutLink;
        protected Label UserNameLabel;

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            string displayName = UserDisplayName;
            if (displayName != "Guest")
            {
                if (LoginLink != null) LoginLink.Visible = false;
                if (LogoutLink != null) LogoutLink.Visible = true;
                if (UserNameLabel != null) UserNameLabel.Text = "Welcome, " + displayName + " | ";
            }
            else
            {
                if (LoginLink != null) LoginLink.Visible = true;
                if (LogoutLink != null) LogoutLink.Visible = false;
                if (UserNameLabel != null) UserNameLabel.Text = "";
            }
        }

        protected void LogoutLink_Click(object sender, EventArgs e)
        {
            Session.Clear();
            Response.Redirect("~/Default.aspx");
        }
    }
}
