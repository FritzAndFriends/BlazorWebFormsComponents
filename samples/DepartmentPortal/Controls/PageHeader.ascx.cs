using System;
using System.Web.UI;

namespace DepartmentPortal.Controls
{
    public partial class PageHeader : BaseUserControl
    {
        protected System.Web.UI.WebControls.Literal litPageTitle;
        protected System.Web.UI.HtmlControls.HtmlGenericControl pnlUserInfo;
        protected System.Web.UI.WebControls.Literal litUserName;

        public string PageTitle
        {
            get { return (string)ViewState["PageTitle"] ?? string.Empty; }
            set { ViewState["PageTitle"] = value; }
        }

        public bool ShowUserInfo
        {
            get
            {
                object val = ViewState["ShowUserInfo"];
                return val != null ? (bool)val : false;
            }
            set { ViewState["ShowUserInfo"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            litPageTitle.Text = PageTitle;

            if (ShowUserInfo)
            {
                pnlUserInfo.Visible = true;
                string userName = Session["UserName"] as string ?? "Guest";
                litUserName.Text = Server.HtmlEncode(userName);
            }

            LogActivity("PageHeader rendered: " + PageTitle);
        }
    }
}
