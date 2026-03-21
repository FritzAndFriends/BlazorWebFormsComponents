using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DepartmentPortal.Models;

namespace DepartmentPortal.Controls
{
    public partial class TrainingCatalog : BaseUserControl
    {
        protected Repeater rptCourses;

        public event EventHandler<int> EnrollmentRequested;

        public IEnumerable<TrainingCourse> Courses { get; set; }

        public bool ShowEnrolled
        {
            get
            {
                object val = ViewState["ShowEnrolled"];
                return val != null ? (bool)val : false;
            }
            set { ViewState["ShowEnrolled"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Courses != null)
            {
                rptCourses.DataSource = Courses.ToList();
                rptCourses.DataBind();
            }

            LogActivity("TrainingCatalog rendered, ShowEnrolled=" + ShowEnrolled);
        }

        protected void rptCourses_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            if (e.CommandName == "Enroll")
            {
                int courseId = int.Parse(e.CommandArgument.ToString());
                OnEnrollmentRequested(courseId);
            }
        }

        protected virtual void OnEnrollmentRequested(int courseId)
        {
            EnrollmentRequested?.Invoke(this, courseId);
            LogActivity("Enrollment requested for course ID: " + courseId);
        }
    }
}
