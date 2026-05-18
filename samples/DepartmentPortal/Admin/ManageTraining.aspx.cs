using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DepartmentPortal.Models;

namespace DepartmentPortal.Admin
{
    public partial class ManageTrainingPage : BasePage
    {
        protected Label EditCoursePanelTitle;
        protected HiddenField EditCourseId;
        protected Panel EditCoursePanel;
        protected GridView CoursesGridView;
        protected TextBox CourseNameTextBox;
        protected TextBox DescriptionTextBox;
        protected TextBox CategoryTextBox;
        protected TextBox DurationTextBox;
        protected TextBox InstructorTextBox;
        protected CheckBox IsAvailableCheckBox;
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
        
        protected void AddNewCourseButton_Click(object sender, EventArgs e)
        {
            EditCoursePanelTitle.Text = "Add New Course";
            EditCourseId.Value = "0";
            ClearEditForm();
            EditCoursePanel.Visible = true;
        }
        
        protected void CoursesGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int courseId = Convert.ToInt32(e.CommandArgument);
            
            if (e.CommandName == "EditCourse")
            {
                LoadCourseForEdit(courseId);
            }
            else if (e.CommandName == "DeleteCourse")
            {
                // In a real app, this would delete from database
                ShowMessage("Course deleted successfully.");
                BindGrid();
            }
        }
        
        protected void SaveCourseButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            
            int courseId = Convert.ToInt32(EditCourseId.Value);
            
            // In a real app, this would save to database
            string message = courseId == 0 ? "Course created successfully." : "Course updated successfully.";
            ShowMessage(message);
            
            EditCoursePanel.Visible = false;
            BindGrid();
        }
        
        protected void CancelCourseButton_Click(object sender, EventArgs e)
        {
            EditCoursePanel.Visible = false;
            ClearEditForm();
        }
        
        private void BindGrid()
        {
            var courses = PortalDataProvider.GetCourses();
            CoursesGridView.DataSource = courses;
            CoursesGridView.DataBind();
        }
        
        private void LoadCourseForEdit(int courseId)
        {
            var course = PortalDataProvider.GetCourses().FirstOrDefault(c => c.Id == courseId);
            
            if (course == null)
                return;
            
            EditCoursePanelTitle.Text = "Edit Course";
            EditCourseId.Value = course.Id.ToString();
            CourseNameTextBox.Text = course.CourseName;
            DescriptionTextBox.Text = course.Description;
            CategoryTextBox.Text = course.Category;
            DurationTextBox.Text = course.DurationHours.ToString();
            InstructorTextBox.Text = course.Instructor;
            IsAvailableCheckBox.Checked = true;
            EditCoursePanel.Visible = true;
        }
        
        private void ClearEditForm()
        {
            CourseNameTextBox.Text = string.Empty;
            DescriptionTextBox.Text = string.Empty;
            CategoryTextBox.Text = string.Empty;
            DurationTextBox.Text = "0";
            InstructorTextBox.Text = string.Empty;
            IsAvailableCheckBox.Checked = true;
        }
    }
}
