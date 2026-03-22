using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using DepartmentPortal.Models;

namespace DepartmentPortal.Controls
{
    public partial class EmployeeList : BaseUserControl
    {
        protected GridView gvEmployees;
        public IEnumerable<Employee> Employees { get; set; }

        public string DepartmentFilter
        {
            get { return (string)ViewState["DepartmentFilter"] ?? string.Empty; }
            set { ViewState["DepartmentFilter"] = value; }
        }

        public int PageSize
        {
            get
            {
                object val = ViewState["PageSize"];
                return val != null ? (int)val : 10;
            }
            set { ViewState["PageSize"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            gvEmployees.PageSize = PageSize;
            BindGrid();
        }

        private void BindGrid()
        {
            var data = Employees;
            if (data == null) return;

            if (!string.IsNullOrEmpty(DepartmentFilter))
            {
                data = data.Where(emp =>
                    emp.Department.Equals(DepartmentFilter, StringComparison.OrdinalIgnoreCase));
            }

            gvEmployees.DataSource = data.ToList();
            gvEmployees.DataBind();

            LogActivity("EmployeeList bound with filter: " + DepartmentFilter);
        }

        protected void gvEmployees_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            gvEmployees.PageIndex = e.NewPageIndex;
            BindGrid();
        }
    }
}
