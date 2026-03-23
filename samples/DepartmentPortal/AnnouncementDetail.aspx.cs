using System;
using System.Linq;
using System.Web.UI.WebControls;
using DepartmentPortal.Models;

namespace DepartmentPortal
{
    public partial class AnnouncementDetailPage : BasePage
    {
        protected Panel AnnouncementDetailsPanel;
        protected Panel NotFoundPanel;
        protected Label TitleLabel;
        protected Label PublishDateLabel;
        protected Label AuthorLabel;
        protected Label BodyLabel;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int announcementId = 0;
                if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out announcementId))
                {
                    LoadAnnouncement(announcementId);
                }
                else
                {
                    ShowNotFound();
                }
            }
        }
        
        private void LoadAnnouncement(int announcementId)
        {
            var announcement = PortalDataProvider.GetAnnouncements()
                .FirstOrDefault(a => a.Id == announcementId && a.IsActive);
            
            if (announcement == null)
            {
                ShowNotFound();
                return;
            }
            
            AnnouncementDetailsPanel.Visible = true;
            NotFoundPanel.Visible = false;
            
            // Set page header
            var pageHeader = (DepartmentPortal.Controls.PageHeader)FindControl("PageHeaderControl");
            if (pageHeader != null)
            {
                pageHeader.PageTitle = announcement.Title;
            }
            
            // Set content
            TitleLabel.Text = announcement.Title;
            PublishDateLabel.Text = announcement.PublishDate.ToString("MMMM d, yyyy");
            AuthorLabel.Text = announcement.Author;
            BodyLabel.Text = announcement.Body;
        }
        
        private void ShowNotFound()
        {
            AnnouncementDetailsPanel.Visible = false;
            NotFoundPanel.Visible = true;
        }
    }
}
