using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DepartmentPortal.Models;

namespace DepartmentPortal
{
    public partial class TrainingPage : BasePage
    {
        protected Label EnrollmentCountLabel;
        private string SearchQuery
        {
            get { return ViewState["SearchQuery"] as string ?? string.Empty; }
            set { ViewState["SearchQuery"] = value; }
        }
        
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
                BindTrainingCatalog();
                UpdateEnrollmentCount();
                SetupPoll();
            }
        }
        
        protected void SearchBoxControl_Search(object sender, SearchEventArgs e)
        {
            SearchQuery = e.SearchTerm;
            BindTrainingCatalog();
        }
        
        protected void TrainingCatalogControl_EnrollmentRequested(object sender, int courseId)
        {
            // In a real app, this would get the course ID from the event
            // For now, just show a message
            ShowMessage("Successfully enrolled in course!");
            UpdateEnrollmentCount();
        }
        
        protected void PollQuestionControl_AnswerSubmitted(object sender, DepartmentPortal.Controls.PollVoteEventArgs e)
        {
            ShowMessage("Thank you for your feedback!");
        }
        
        private void BindTrainingCatalog()
        {
            var allCourses = PortalDataProvider.GetCourses();
            
            // Apply search filter
            var filteredCourses = allCourses.AsEnumerable();
            
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                filteredCourses = filteredCourses.Where(c => 
                    c.CourseName.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    c.Category.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    c.Description.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            
            var catalog = (DepartmentPortal.Controls.TrainingCatalog)FindControl("TrainingCatalogControl");
            if (catalog != null)
            {
                catalog.Courses = filteredCourses.ToList();
            }
        }
        
        private void UpdateEnrollmentCount()
        {
            EnrollmentCountLabel.Text = EnrolledCourses.Count.ToString();
        }
        
        private void SetupPoll()
        {
            var poll = (DepartmentPortal.Controls.PollQuestion)FindControl("PollQuestionControl");
            if (poll != null)
            {
                poll.Options = "In-person classroom,Live virtual sessions,Self-paced online,Hybrid (mix of all)";
            }
        }
    }
}
