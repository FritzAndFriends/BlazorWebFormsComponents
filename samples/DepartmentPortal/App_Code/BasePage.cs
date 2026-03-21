using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using DepartmentPortal.Models;

namespace DepartmentPortal
{
    public class BasePage : Page
    {
        public Employee CurrentUser { get; private set; }

        public bool IsAdmin
        {
            get { return CurrentUser != null && CurrentUser.IsAdmin; }
        }

        protected override void OnPreInit(EventArgs e)
        {
            MasterPageFile = "~/Site.Master";
            base.OnPreInit(e);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Session["UserId"] == null)
            {
                Response.Redirect("~/Login.aspx");
                return;
            }

            int userId = (int)Session["UserId"];
            CurrentUser = PortalDataProvider.GetEmployees()
                .FirstOrDefault(emp => emp.Id == userId);
        }

        protected void ShowMessage(string message)
        {
            if (Master != null)
            {
                var messageLiteral = Master.FindControl("MessageLiteral") as System.Web.UI.WebControls.Literal;
                if (messageLiteral != null)
                {
                    messageLiteral.Text = "<div class='alert alert-info'>" +
                        HttpUtility.HtmlEncode(message) + "</div>";
                }
            }
        }
    }
}
