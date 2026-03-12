// =============================================================================
// TODO: This code-behind was copied from Web Forms and needs manual migration.
//
// Common transforms needed (use the BWFC Copilot skill for assistance):
//   - Page_Load / Page_Init → OnInitializedAsync / OnParametersSetAsync
//   - Page_PreRender → OnAfterRenderAsync
//   - IsPostBack checks → remove or convert to state logic
//   - ViewState usage → component [Parameter] or private fields
//   - Session/Cache access → inject IHttpContextAccessor or use DI
//   - Response.Redirect → NavigationManager.NavigateTo
//   - Event handlers (Button_Click, etc.) → convert to Blazor event callbacks
//   - Data binding (DataBind, DataSource) → component parameters or OnInitialized
//   - UpdatePanel / ScriptManager references → remove (Blazor handles updates)
//   - User controls → Blazor component references
// =============================================================================
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
