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
        protected DepartmentPortal.Controls.TrainingCatalog TrainingCatalogControl;
        protected DepartmentPortal.Controls.PollQuestion PollQuestionControl;

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
                SetupPoll();
            }
        }
        
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            BindTrainingCatalog();
            UpdateEnrollmentCount();
        }
        
        protected void SearchBoxControl_Search(object sender, SearchEventArgs e)
        {
            SearchQuery = e.SearchTerm;
        }
        
        protected void TrainingCatalogControl_EnrollmentRequested(object sender, int courseId)
        {
            ShowMessage("Successfully enrolled in course!");
        }
        
        protected void PollQuestionControl_AnswerSubmitted(object sender, DepartmentPortal.Controls.PollVoteEventArgs e)
        {
            ShowMessage("Thank you for your feedback!");
        }
        
        private void BindTrainingCatalog()
        {
            var allCourses = PortalDataProvider.GetCourses();
            
            var filteredCourses = allCourses.AsEnumerable();
            
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                filteredCourses = filteredCourses.Where(c => 
                    c.CourseName.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    c.Category.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    c.Description.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            
            if (TrainingCatalogControl != null)
            {
                TrainingCatalogControl.Courses = filteredCourses.ToList();
            }
        }
        
        private void UpdateEnrollmentCount()
        {
            if (EnrollmentCountLabel != null)
            {
                EnrollmentCountLabel.Text = EnrolledCourses.Count.ToString();
            }
        }
        
        private void SetupPoll()
        {
            if (PollQuestionControl != null)
            {
                PollQuestionControl.Options = "In-person classroom,Live virtual sessions,Self-paced online,Hybrid (mix of all)";
            }
        }
    }
}
