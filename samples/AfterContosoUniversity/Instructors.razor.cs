using System;
using System.Collections.Generic;
using System.Linq;
using ContosoUniversity.Models;
using ContosoUniversity.BLL;

namespace ContosoUniversity
{
    public partial class Instructors : WebFormsPageBase
    {
    // TODO(bwfc-general): ClientScript calls preserved — works via WebFormsPageBase (no injection needed). ScriptManagerShim may need @inject ScriptManagerShim ScriptManager for non-page classes.

    // --- Request.Form Migration ---
    // TODO(bwfc-form): Request.Form calls work automatically via RequestShim on WebFormsPageBase.
    // For interactive mode, wrap your form in <WebFormsForm OnSubmit="SetRequestFormData">.
    // Form keys found: key
    // For non-page classes, inject RequestShim via DI.
 
    private GridView<object> grvInstructors = default!;     
    private object _grvInstructors_DataSource = null!;
        private Instructors_Logic instructorsLogic;

        [Inject]
        protected ContosoUniversityEntities _contosoUniversityEntities { get; set; } = default!;
             
        protected override async Task OnInitializedAsync()
        {
            // TODO(bwfc-lifecycle): Review lifecycle conversion — verify async behavior
            await base.OnInitializedAsync();
         
            instructorsLogic = new Instructors_Logic(_contosoUniversityEntities);              
            _grvInstructors_DataSource = instructorsLogic.getInstructors();

                        // BWFC: IsPostBack guard unwrapped — Blazor re-renders on every state change
            ViewState["SortDirection"] = "desc";
        }

        #region Sorting Grid
        protected void grvInstructors_Sorting(GridViewSortEventArgs e)
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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // TODO(bwfc-lifecycle): Review lifecycle conversion — verify async behavior
            if (firstRender)
            {
            }
        }     
    }
}
