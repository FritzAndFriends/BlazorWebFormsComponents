using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DepartmentPortal.Models;

namespace DepartmentPortal.Controls
{
    public partial class DepartmentFilter : BaseUserControl
    {
        protected DropDownList ddlDepartments;
        public event EventHandler DepartmentChanged;

        public int SelectedDepartmentId
        {
            get
            {
                object val = ViewState["SelectedDepartmentId"];
                return val != null ? (int)val : 0;
            }
            set { ViewState["SelectedDepartmentId"] = value; }
        }

        public bool AutoPostBack
        {
            get
            {
                object val = ViewState["AutoPostBack"];
                return val != null ? (bool)val : false;
            }
            set { ViewState["AutoPostBack"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            ddlDepartments.AutoPostBack = AutoPostBack;

            if (!IsPostBack)
            {
                var departments = PortalDataProvider.GetDepartments();
                ddlDepartments.Items.Clear();
                ddlDepartments.Items.Add(new System.Web.UI.WebControls.ListItem("-- All Departments --", "0"));

                foreach (var dept in departments)
                {
                    ddlDepartments.Items.Add(
                        new System.Web.UI.WebControls.ListItem(dept.Name, dept.Id.ToString()));
                }

                if (SelectedDepartmentId > 0)
                {
                    ddlDepartments.SelectedValue = SelectedDepartmentId.ToString();
                }

                LogActivity("DepartmentFilter loaded with " + departments.Count + " departments");
            }
        }

        protected void ddlDepartments_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectedDepartmentId = int.Parse(ddlDepartments.SelectedValue);
            OnDepartmentChanged(EventArgs.Empty);
        }

        protected virtual void OnDepartmentChanged(EventArgs args)
        {
            DepartmentChanged?.Invoke(this, args);
            LogActivity("Department changed to ID: " + SelectedDepartmentId);
        }
    }
}
