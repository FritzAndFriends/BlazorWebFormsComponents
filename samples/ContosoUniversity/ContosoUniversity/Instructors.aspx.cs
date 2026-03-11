using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ContosoUniversity.Models;
using ContosoUniversity.BLL;

namespace ContosoUniversity
{
    public partial class Instructors : System.Web.UI.Page
    {     
        private Instructors_Logic instructorsLogic;
            
        protected void Page_Load(object sender, EventArgs e)
        {         
            instructorsLogic = new Instructors_Logic();              
            this.grvInstructors.DataSource = instructorsLogic.getInstructors();

            if (!IsPostBack)
            {
                ViewState["SortDirection"] = "desc";
            }
        }

        #region Sorting Grid
        protected void grvInstructors_Sorting(object sender, GridViewSortEventArgs e)
        {
            this.grvInstructors.DataSource = instructorsLogic.GetSortedInstrucors(e.SortExpression, ViewState["SortDirection"].ToString());
            ChangeSortDirection();
        }

        private void ChangeSortDirection()
        {
            if (ViewState["SortDirection"].ToString() == "asc")
            {
                ViewState["SortDirection"] = "desc";
            }
            else
            {
                ViewState["SortDirection"] = "asc";
            }
        }
        #endregion

        protected void Page_PreRender(object sender, EventArgs e)
        {
            this.grvInstructors.DataBind();
        }     
    }
}