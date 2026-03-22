using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DepartmentPortal.Controls
{
    public partial class DashboardWidget : BaseUserControl
    {
        protected Literal litWidgetTitle;
        protected Literal litIcon;
        protected PlaceHolder phContent;
        public string WidgetTitle
        {
            get { return (string)ViewState["WidgetTitle"] ?? string.Empty; }
            set { ViewState["WidgetTitle"] = value; }
        }

        public string IconClass
        {
            get { return (string)ViewState["IconClass"] ?? string.Empty; }
            set { ViewState["IconClass"] = value; }
        }

        /// <summary>
        /// Exposes the PlaceHolder for parent pages to add custom content.
        /// </summary>
        public PlaceHolder ContentPlaceHolder
        {
            get { return phContent; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            litWidgetTitle.Text = Server.HtmlEncode(WidgetTitle);

            if (!string.IsNullOrEmpty(IconClass))
            {
                litIcon.Text = string.Format("<i class=\"{0}\"></i>", Server.HtmlEncode(IconClass));
            }

            LogActivity("DashboardWidget rendered: " + WidgetTitle);
        }
    }
}
