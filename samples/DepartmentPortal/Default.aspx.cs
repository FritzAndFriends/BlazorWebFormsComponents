using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DepartmentPortal.Models;

namespace DepartmentPortal
{
    public partial class DefaultPage : Page
    {
        protected Panel LoggedOutPanel;
        protected Panel LoggedInPanel;
        protected Label EmployeeCountLabel;
        protected Label AnnouncementCountLabel;
        protected Label CourseCountLabel;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["UserId"] != null)
            {
                LoggedOutPanel.Visible = false;
                LoggedInPanel.Visible = true;

                EmployeeCountLabel.Text = PortalDataProvider.GetEmployees().Count.ToString();
                AnnouncementCountLabel.Text = PortalDataProvider.GetAnnouncements()
                    .Count(a => a.IsActive).ToString();
                CourseCountLabel.Text = PortalDataProvider.GetCourses().Count.ToString();
            }
            else
            {
                LoggedOutPanel.Visible = true;
                LoggedInPanel.Visible = false;
            }
        }
    }
}
