using System;
using System.Web.UI.WebControls;

namespace DepartmentPortal
{
    public partial class SiteMaster : BaseMasterPage
    {
        protected Label UserNameLabel;

        protected new void Page_Load(object sender, EventArgs e)
        {
            base.Page_Load(sender, e);

            if (UserNameLabel != null)
            {
                string displayName = UserDisplayName;
                if (displayName != "Guest")
                {
                    UserNameLabel.Text = "Welcome, " + displayName;
                }
                else
                {
                    UserNameLabel.Text = "";
                }
            }
        }
    }
}
