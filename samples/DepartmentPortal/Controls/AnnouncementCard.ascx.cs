using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using DepartmentPortal.Models;

namespace DepartmentPortal.Controls
{
    public partial class AnnouncementCard : BaseUserControl
    {
        protected Literal litTitle;
        protected Literal litDate;
        protected Literal litAuthor;
        protected Literal litBody;
        public Announcement Announcement { get; set; }

        public bool ShowFullText
        {
            get
            {
                object val = ViewState["ShowFullText"];
                return val != null ? (bool)val : false;
            }
            set { ViewState["ShowFullText"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Announcement != null)
            {
                litTitle.Text = HttpUtility.HtmlEncode(Announcement.Title);
                litDate.Text = Announcement.PublishDate.ToString("MMMM dd, yyyy");
                litAuthor.Text = HttpUtility.HtmlEncode(Announcement.Author);

                if (ShowFullText)
                {
                    litBody.Text = HttpUtility.HtmlEncode(Announcement.Body);
                }
                else
                {
                    string body = Announcement.Body ?? string.Empty;
                    string summary = body.Length > 150 ? body.Substring(0, 150) + "..." : body;
                    litBody.Text = HttpUtility.HtmlEncode(summary);
                }

                LogActivity("AnnouncementCard rendered: " + Announcement.Title);
            }
        }
    }
}
