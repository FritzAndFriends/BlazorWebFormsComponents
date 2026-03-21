using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;

namespace DepartmentPortal.Controls
{
    public partial class Breadcrumb : BaseUserControl
    {
        protected System.Web.UI.HtmlControls.HtmlGenericControl homeLinkItem;
        protected System.Web.UI.WebControls.Repeater rptBreadcrumb;

        public string CurrentPath
        {
            get { return (string)ViewState["CurrentPath"] ?? string.Empty; }
            set { ViewState["CurrentPath"] = value; }
        }

        public bool ShowHomeLink
        {
            get
            {
                object val = ViewState["ShowHomeLink"];
                return val != null ? (bool)val : true;
            }
            set { ViewState["ShowHomeLink"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            homeLinkItem.Visible = ShowHomeLink;

            if (!string.IsNullOrEmpty(CurrentPath))
            {
                var segments = CurrentPath.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                rptBreadcrumb.DataSource = segments;
                rptBreadcrumb.DataBind();
            }

            LogActivity("Breadcrumb rendered for path: " + CurrentPath);
        }
    }
}
