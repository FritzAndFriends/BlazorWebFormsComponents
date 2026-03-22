using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using DepartmentPortal.Models;

namespace DepartmentPortal
{
    public partial class EmployeesPage : BasePage
    {
        protected Label EmployeeCountLabel;
        protected DepartmentPortal.Controls.Pager PagerControl;
        protected DepartmentPortal.Controls.EmployeeDataGrid EmployeeDataGridControl;
        private const int PageSize = 12;
        private int CurrentPageIndex
        {
            get { return ViewState["CurrentPageIndex"] != null ? (int)ViewState["CurrentPageIndex"] : 0; }
            set { ViewState["CurrentPageIndex"] = value; }
        }
        
        private string SearchQuery
        {
            get { return ViewState["SearchQuery"] as string ?? string.Empty; }
            set { ViewState["SearchQuery"] = value; }
        }
        
        private int SelectedDepartmentId
        {
            get { return ViewState["SelectedDepartmentId"] != null ? (int)ViewState["SelectedDepartmentId"] : -1; }
            set { ViewState["SelectedDepartmentId"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
        
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            BindEmployees();
        }
        
        protected void SearchBoxControl_Search(object sender, SearchEventArgs e)
        {
            SearchQuery = e.SearchTerm;
            CurrentPageIndex = 0;
        }
        
        protected void DepartmentFilterControl_DepartmentChanged(object sender, EventArgs e)
        {
            var filter = (DepartmentPortal.Controls.DepartmentFilter)sender;
            SelectedDepartmentId = filter.SelectedDepartmentId;
            CurrentPageIndex = 0;
        }
        
        protected void PagerControl_PageChanged(object sender, int pageNumber)
        {
            CurrentPageIndex = pageNumber - 1;
        }
        
        private void BindEmployees()
        {
            var allEmployees = PortalDataProvider.GetEmployees();
            
            // Apply filters
            var filteredEmployees = allEmployees.AsEnumerable();
            
            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                filteredEmployees = filteredEmployees.Where(e => 
                    e.Name.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    e.Title.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    e.Email.IndexOf(SearchQuery, StringComparison.OrdinalIgnoreCase) >= 0);
            }
            
            if (SelectedDepartmentId > 0)
            {
                var dept = PortalDataProvider.GetDepartments().FirstOrDefault(d => d.Id == SelectedDepartmentId);
                if (dept != null)
                {
                    filteredEmployees = filteredEmployees.Where(e => e.Department == dept.Name);
                }
            }
            
            var employeeList = filteredEmployees.ToList();
            if (EmployeeCountLabel != null)
            {
                EmployeeCountLabel.Text = employeeList.Count.ToString();
            }
            
            // Set up pager
            if (PagerControl != null)
            {
                PagerControl.TotalPages = (int)Math.Ceiling((double)employeeList.Count / PageSize);
                PagerControl.CurrentPage = CurrentPageIndex + 1;
            }
            
            // Get page of employees
            var pagedEmployees = employeeList
                .Skip(CurrentPageIndex * PageSize)
                .Take(PageSize)
                .ToList();
            
            // Bind to custom EmployeeDataGrid control
            if (EmployeeDataGridControl != null)
            {
                EmployeeDataGridControl.DataSource = pagedEmployees;
                EmployeeDataGridControl.DataBind();
            }
        }
    }
}
