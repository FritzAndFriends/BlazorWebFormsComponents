using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DepartmentPortal.Models;

namespace DepartmentPortal
{
    public partial class MyTrainingPage : BasePage
    {
        protected Panel EnrolledCoursesPanel;
        protected Panel NoCoursesPanel;
        protected Label EnrolledCountLabel;
        private List<int> EnrolledCourses
        {
            get
            {
                if (Session["EnrolledCourses"] == null)
                {
                    Session["EnrolledCourses"] = new List<int>();
                }
                return (List<int>)Session["EnrolledCourses"];
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindEnrolledCourses();
            }
        }
        
        private void BindEnrolledCourses()
        {
            var enrolledCourseIds = EnrolledCourses;
            
            if (enrolledCourseIds.Count == 0)
            {
                EnrolledCoursesPanel.Visible = false;
                NoCoursesPanel.Visible = true;
                return;
            }
            
            var allCourses = PortalDataProvider.GetCourses();
            var enrolledCourses = allCourses.Where(c => enrolledCourseIds.Contains(c.Id)).ToList();
            
            EnrolledCoursesPanel.Visible = true;
            NoCoursesPanel.Visible = false;
            
            EnrolledCountLabel.Text = enrolledCourses.Count.ToString();
            
            var catalog = (DepartmentPortal.Controls.TrainingCatalog)FindControl("EnrolledTrainingCatalogControl");
            if (catalog != null)
            {
                catalog.Courses = enrolledCourses;
            }
        }
    }
}
