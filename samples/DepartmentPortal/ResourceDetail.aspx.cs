using System;
using System.Linq;
using System.Web.UI.WebControls;
using DepartmentPortal.Models;

namespace DepartmentPortal
{
    public partial class ResourceDetailPage : BasePage
    {
        protected Panel ResourceDetailsPanel;
        protected Panel NotFoundPanel;
        protected Label TitleLabel;
        protected Label CategoryLabel;
        protected Label DescriptionLabel;
        protected Label FileTypeLabel;
        protected Label FileSizeLabel;
        protected Label LastUpdatedLabel;
        protected HyperLink DownloadLink;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int resourceId = 0;
                if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out resourceId))
                {
                    LoadResource(resourceId);
                }
                else
                {
                    ShowNotFound();
                }
            }
        }
        
        protected void ShareButton_Click(object sender, EventArgs e)
        {
            ShowMessage("Share functionality coming soon!");
        }
        
        private void LoadResource(int resourceId)
        {
            var resource = PortalDataProvider.GetResources().FirstOrDefault(r => r.Id == resourceId);
            
            if (resource == null)
            {
                ShowNotFound();
                return;
            }
            
            ResourceDetailsPanel.Visible = true;
            NotFoundPanel.Visible = false;
            
            // Set page header
            var pageHeader = (DepartmentPortal.Controls.PageHeader)FindControl("PageHeaderControl");
            if (pageHeader != null)
            {
                pageHeader.PageTitle = resource.Title;
            }
            
            // Set content
            TitleLabel.Text = resource.Title;
            CategoryLabel.Text = resource.CategoryName;
            DescriptionLabel.Text = resource.Description;
            FileTypeLabel.Text = resource.FileType ?? "N/A";
            FileSizeLabel.Text = "N/A";
            LastUpdatedLabel.Text = "N/A";
            
            // Set download link
            DownloadLink.NavigateUrl = resource.Url ?? "#";
        }
        
        private void ShowNotFound()
        {
            ResourceDetailsPanel.Visible = false;
            NotFoundPanel.Visible = true;
        }
        
        private string FormatFileSize(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB" };
            double len = bytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len = len / 1024;
            }
            return String.Format("{0:0.##} {1}", len, sizes[order]);
        }
    }
}
