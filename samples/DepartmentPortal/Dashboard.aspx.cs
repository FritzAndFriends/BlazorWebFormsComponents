using System;
using System.Linq;
using System.Web.UI.WebControls;
using DepartmentPortal.Models;

namespace DepartmentPortal
{
    public partial class DashboardPage : BasePage
    {
        protected Label WelcomeNameLabel;
        protected Repeater RecentAnnouncementsRepeater;
        protected Repeater RecentCoursesRepeater;
        protected Label StatEmployeesLabel;
        protected Label StatDeptLabel;
        protected Label StatCoursesLabel;
        protected Label StatResourcesLabel;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                WelcomeNameLabel.Text = CurrentUser != null ? CurrentUser.Name : "User";

                RecentAnnouncementsRepeater.DataSource = PortalDataProvider.GetAnnouncements()
                    .Where(a => a.IsActive)
                    .OrderByDescending(a => a.PublishDate)
                    .Take(5);
                RecentAnnouncementsRepeater.DataBind();

                RecentCoursesRepeater.DataSource = PortalDataProvider.GetCourses().Take(5);
                RecentCoursesRepeater.DataBind();

                StatEmployeesLabel.Text = PortalDataProvider.GetEmployees().Count.ToString();
                StatDeptLabel.Text = PortalDataProvider.GetDepartments().Count.ToString();
                StatCoursesLabel.Text = PortalDataProvider.GetCourses().Count.ToString();
                StatResourcesLabel.Text = PortalDataProvider.GetResources().Count.ToString();
            }
        }
    }
}
