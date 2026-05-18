using System;
using System.Linq;
using System.Web.UI.WebControls;
using DepartmentPortal.Models;

namespace DepartmentPortal
{
    public partial class EmployeeDetailPage : BasePage
    {
        protected Panel EmployeeDetailsPanel;
        protected Panel NotFoundPanel;
        protected Label EmailLabel;
        protected Label PhoneLabel;
        protected Label DepartmentLabel;
        protected Label HireDateLabel;
        protected HyperLink SendEmailLink;
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int employeeId = 0;
                if (Request.QueryString["id"] != null && int.TryParse(Request.QueryString["id"], out employeeId))
                {
                    LoadEmployee(employeeId);
                }
                else
                {
                    ShowNotFound();
                }
            }
        }
        
        private void LoadEmployee(int employeeId)
        {
            var employee = PortalDataProvider.GetEmployees().FirstOrDefault(e => e.Id == employeeId);
            
            if (employee == null)
            {
                ShowNotFound();
                return;
            }
            
            EmployeeDetailsPanel.Visible = true;
            NotFoundPanel.Visible = false;
            
            // Set page header
            var pageHeader = (DepartmentPortal.Controls.PageHeader)FindControl("PageHeaderControl");
            if (pageHeader != null)
            {
                pageHeader.PageTitle = employee.Name;
            }
            
            // Set employee card
            var employeeCard = (DepartmentPortal.Controls.EmployeeCard)FindControl("EmployeeCardControl");
            if (employeeCard != null)
            {
                employeeCard.EmployeeId = employee.Id;
            }
            
            // Set contact info
            EmailLabel.Text = employee.Email;
            PhoneLabel.Text = employee.Phone;
            DepartmentLabel.Text = employee.Department;
            HireDateLabel.Text = employee.HireDate.ToString("MMMM d, yyyy");
            
            // Set performance rating (4 stars for demonstration)
            var ratingControl = (DepartmentPortal.Controls.StarRating)FindControl("PerformanceRatingControl");
            if (ratingControl != null)
            {
                ratingControl.Rating = 4;
            }
            
            // Set email link
            SendEmailLink.NavigateUrl = "mailto:" + employee.Email;
        }
        
        private void ShowNotFound()
        {
            EmployeeDetailsPanel.Visible = false;
            NotFoundPanel.Visible = true;
        }
    }
}
