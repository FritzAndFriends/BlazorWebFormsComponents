using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DepartmentPortal.Models;

namespace DepartmentPortal
{
    public partial class LoginPage : Page
    {
        protected DropDownList UserDropDown;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                UserDropDown.DataSource = PortalDataProvider.GetEmployees();
                UserDropDown.DataBind();
            }
        }

        protected void LoginButton_Click(object sender, EventArgs e)
        {
            int userId = int.Parse(UserDropDown.SelectedValue);
            var employee = PortalDataProvider.GetEmployees()
                .FirstOrDefault(emp => emp.Id == userId);

            if (employee != null)
            {
                Session["UserId"] = employee.Id;
                Session["UserName"] = employee.Name;
                Response.Redirect("~/Dashboard.aspx");
            }
        }
    }
}
