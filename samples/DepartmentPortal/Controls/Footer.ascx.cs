using System;
using System.Web.UI;

namespace DepartmentPortal.Controls
{
    public partial class Footer : BaseUserControl
    {
        protected System.Web.UI.WebControls.Literal litYear;
        protected System.Web.UI.HtmlControls.HtmlGenericControl pnlLinks;

        public bool ShowLinks
        {
            get
            {
                object val = ViewState["ShowLinks"];
                return val != null ? (bool)val : true;
            }
            set { ViewState["ShowLinks"] = value; }
        }

        public int Year
        {
            get
            {
                object val = ViewState["Year"];
                return val != null ? (int)val : DateTime.Now.Year;
            }
            set { ViewState["Year"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            litYear.Text = Year.ToString();
            pnlLinks.Visible = ShowLinks;

            LogActivity("Footer rendered for year: " + Year);
        }
    }
}
