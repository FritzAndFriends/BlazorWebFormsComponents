using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DepartmentPortal.Models;

namespace DepartmentPortal.Admin
{
    public partial class ManageAnnouncementsPage : BasePage
    {
        protected Label EditPanelTitle;
        protected HiddenField EditAnnouncementId;
        protected Panel EditPanel;
        protected GridView AnnouncementsGridView;
        protected TextBox TitleTextBox;
        protected TextBox BodyTextBox;
        protected TextBox AuthorTextBox;
        protected TextBox PublishDateTextBox;
        protected CheckBox IsActiveCheckBox;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsAdmin)
            {
                ShowMessage("Access denied. Administrator privileges required.");
                Response.Redirect("~/Dashboard.aspx");
                return;
            }
            
            if (!IsPostBack)
            {
                BindGrid();
            }
        }
        
        protected void AddNewButton_Click(object sender, EventArgs e)
        {
            EditPanelTitle.Text = "Add New Announcement";
            EditAnnouncementId.Value = "0";
            ClearEditForm();
            EditPanel.Visible = true;
        }
        
        protected void AnnouncementsGridView_RowEditing(object sender, GridViewEditEventArgs e)
        {
            // Handled in RowCommand
        }
        
        protected void AnnouncementsGridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            // Handled in RowCommand
        }
        
        protected void AnnouncementsGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int announcementId = Convert.ToInt32(e.CommandArgument);
            
            if (e.CommandName == "Edit")
            {
                LoadAnnouncementForEdit(announcementId);
            }
            else if (e.CommandName == "Delete")
            {
                // In a real app, this would delete from database
                ShowMessage("Announcement deleted successfully.");
                BindGrid();
            }
        }
        
        protected void SaveButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            
            int announcementId = Convert.ToInt32(EditAnnouncementId.Value);
            
            // In a real app, this would save to database
            string message = announcementId == 0 ? "Announcement created successfully." : "Announcement updated successfully.";
            ShowMessage(message);
            
            EditPanel.Visible = false;
            BindGrid();
        }
        
        protected void CancelButton_Click(object sender, EventArgs e)
        {
            EditPanel.Visible = false;
            ClearEditForm();
        }
        
        private void BindGrid()
        {
            var announcements = PortalDataProvider.GetAnnouncements().OrderByDescending(a => a.PublishDate);
            AnnouncementsGridView.DataSource = announcements;
            AnnouncementsGridView.DataBind();
        }
        
        private void LoadAnnouncementForEdit(int announcementId)
        {
            var announcement = PortalDataProvider.GetAnnouncements().FirstOrDefault(a => a.Id == announcementId);
            
            if (announcement == null)
                return;
            
            EditPanelTitle.Text = "Edit Announcement";
            EditAnnouncementId.Value = announcement.Id.ToString();
            TitleTextBox.Text = announcement.Title;
            BodyTextBox.Text = announcement.Body;
            AuthorTextBox.Text = announcement.Author;
            PublishDateTextBox.Text = announcement.PublishDate.ToString("yyyy-MM-dd");
            IsActiveCheckBox.Checked = announcement.IsActive;
            EditPanel.Visible = true;
        }
        
        private void ClearEditForm()
        {
            TitleTextBox.Text = string.Empty;
            BodyTextBox.Text = string.Empty;
            AuthorTextBox.Text = CurrentUser != null ? CurrentUser.Name : string.Empty;
            PublishDateTextBox.Text = DateTime.Now.ToString("yyyy-MM-dd");
            IsActiveCheckBox.Checked = true;
        }
    }
}
