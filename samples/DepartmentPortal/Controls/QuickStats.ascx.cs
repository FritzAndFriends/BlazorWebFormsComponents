using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DepartmentPortal.Models;

namespace DepartmentPortal.Controls
{
    public partial class QuickStats : BaseUserControl
    {
        protected System.Web.UI.HtmlControls.HtmlGenericControl pnlEmployeeCount;
        protected Literal litEmployeeCount;
        protected System.Web.UI.HtmlControls.HtmlGenericControl pnlAnnouncementCount;
        protected Literal litAnnouncementCount;
        public bool ShowEmployeeCount
        {
            get
            {
                object val = ViewState["ShowEmployeeCount"];
                return val != null ? (bool)val : true;
            }
            set { ViewState["ShowEmployeeCount"] = value; }
        }

        public bool ShowAnnouncementCount
        {
            get
            {
                object val = ViewState["ShowAnnouncementCount"];
                return val != null ? (bool)val : true;
            }
            set { ViewState["ShowAnnouncementCount"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (ShowEmployeeCount)
            {
                pnlEmployeeCount.Visible = true;
                litEmployeeCount.Text = PortalDataProvider.GetEmployees().Count.ToString();
            }

            if (ShowAnnouncementCount)
            {
                pnlAnnouncementCount.Visible = true;
                litAnnouncementCount.Text = PortalDataProvider.GetAnnouncements()
                    .Count(a => a.IsActive).ToString();
            }

            LogActivity("QuickStats rendered");
        }
    }
}
