using System;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DepartmentPortal.Models;

namespace DepartmentPortal.Admin
{
    public partial class ManageEmployeesPage : BasePage
    {
        protected Label EmployeeCountLabel;
        protected Label EditEmployeePanelTitle;
        protected HiddenField EditEmployeeId;
        protected Panel EditEmployeePanel;
        protected DropDownList DepartmentDropDownList;
        protected GridView EmployeeGridView;
        protected TextBox NameTextBox;
        protected TextBox EmailTextBox;
        protected TextBox TitleTextBox;
        protected TextBox PhoneTextBox;
        protected TextBox HireDateTextBox;
        private string SearchQuery
        {
            get { return ViewState["SearchQuery"] as string ?? string.Empty; }
            set { ViewState["SearchQuery"] = value; }
        }

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
                BindDepartments();
                BindGrid();
            }
        }
        
        protected void SearchBoxControl_Search(object sender, SearchEventArgs e)
        {
            SearchQuery = e.SearchTerm;
            BindGrid();
        }
        
        protected void AddNewEmployeeButton_Click(object sender, EventArgs e)
        {
            EditEmployeePanelTitle.Text = "Add New Employee";
            EditEmployeeId.Value = "0";
            ClearEditForm();
            EditEmployeePanel.Visible = true;
        }
        
        protected void EmployeeDataGridControl_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            // Handle commands from custom EmployeeDataGrid if needed
        }
        
        protected void EmployeeGridView_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            int employeeId = Convert.ToInt32(e.CommandArgument);
            
            if (e.CommandName == "EditEmployee")
            {
                LoadEmployeeForEdit(employeeId);
            }
        }
        
        protected void SaveEmployeeButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;
            
            int employeeId = Convert.ToInt32(EditEmployeeId.Value);
            
            // In a real app, this would save to database
            string message = employeeId == 0 ? "Employee created successfully." : "Employee updated successfully.";
            ShowMessage(message);
            
            EditEmployeePanel.Visible = false;
            BindGrid();
        }
        
        protected void CancelEmployeeButton_Click(object sender, EventArgs e)
        {
            EditEmployeePanel.Visible = false;
            ClearEditForm();
        }
        
        private void BindGrid()
        {
            var allEmployees = PortalDataProvider.GetEmployees();
            
            // Apply search filter
            var filteredEmployees = allEmployees.AsEnumerable();
            
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                filteredEmployees = filteredEmployees.Where(e => 
                    e.Name.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    e.Title.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    e.Email.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            
            var employeeList = filteredEmployees.ToList();
            EmployeeCountLabel.Text = employeeList.Count.ToString();
            
            EmployeeGridView.DataSource = employeeList;
            EmployeeGridView.DataBind();
            
            // Also bind to custom data grid
            var dataGrid = (DepartmentPortal.Controls.EmployeeDataGrid)FindControl("EmployeeDataGridControl");
            if (dataGrid != null)
            {
                dataGrid.DataSource = employeeList;
                dataGrid.DataBind();
            }
        }
        
        private void BindDepartments()
        {
            var departments = PortalDataProvider.GetDepartments();
            DepartmentDropDownList.DataSource = departments;
            DepartmentDropDownList.DataTextField = "Name";
            DepartmentDropDownList.DataValueField = "Id";
            DepartmentDropDownList.DataBind();
            DepartmentDropDownList.Items.Insert(0, new ListItem("-- Select Department --", "0"));
        }
        
        private void LoadEmployeeForEdit(int employeeId)
        {
            var employee = PortalDataProvider.GetEmployees().FirstOrDefault(e => e.Id == employeeId);
            
            if (employee == null)
                return;
            
            EditEmployeePanelTitle.Text = "Edit Employee";
            EditEmployeeId.Value = employee.Id.ToString();
            NameTextBox.Text = employee.Name;
            EmailTextBox.Text = employee.Email;
            TitleTextBox.Text = employee.Title;
            PhoneTextBox.Text = employee.Phone;
            
            // Find department by name
            var dept = PortalDataProvider.GetDepartments().FirstOrDefault(d => d.Name == employee.Department);
            if (dept != null)
            {
                DepartmentDropDownList.SelectedValue = dept.Id.ToString();
            }
            
            HireDateTextBox.Text = employee.HireDate.ToString("yyyy-MM-dd");
            EditEmployeePanel.Visible = true;
        }
        
        private void ClearEditForm()
        {
            NameTextBox.Text = string.Empty;
            EmailTextBox.Text = string.Empty;
            TitleTextBox.Text = string.Empty;
            PhoneTextBox.Text = string.Empty;
            DepartmentDropDownList.SelectedIndex = 0;
            HireDateTextBox.Text = DateTime.Now.ToString("yyyy-MM-dd");
        }
    }
}
