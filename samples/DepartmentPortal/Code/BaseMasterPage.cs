using System;
using System.Web.UI;

namespace DepartmentPortal
{
    public class BaseMasterPage : MasterPage
    {
        public string UserDisplayName
        {
            get
            {
                if (Session["UserName"] != null)
                {
                    return Session["UserName"].ToString();
                }
                return "Guest";
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
        }
    }
}
