using System;
using System.Collections.Generic;
using System.Linq;
using ContosoUniversity.Models;
using ContosoUniversity.BLL;


namespace ContosoUniversity
{
    public partial class About : WebFormsPageBase
    {
    // TODO(bwfc-general): ClientScript calls preserved — works via WebFormsPageBase (no injection needed). ScriptManagerShim may need @inject ScriptManagerShim ScriptManager for non-page classes.

    // --- Request.Form Migration ---
    // TODO(bwfc-form): Request.Form calls work automatically via RequestShim on WebFormsPageBase.
    // For interactive mode, wrap your form in <WebFormsForm OnSubmit="SetRequestFormData">.
    // Form keys found: key
    // For non-page classes, inject RequestShim via DI.
 
    private GridView<KeyValuePair<string, int>> EnrollmentsStat = default!;
    private List<KeyValuePair<string, int>> _enrollmentData = new();

        [Inject]
        protected ContosoUniversityEntities _contosoUniversityEntities { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            // TODO(bwfc-lifecycle): Review lifecycle conversion — verify async behavior
            await base.OnInitializedAsync();

            _enrollmentData = EnrollmentsStat_GetData().ToList();
        }

        // The return type can be changed to IEnumerable, however to support
        // paging and sorting, the following parameters must be added:
        //     int maximumRows
        //     int startRowIndex
        //     out int totalRowCount
        //     string sortByExpression
        public Dictionary<string, int> EnrollmentsStat_GetData()
        {
            return new Enrollmet_Logic(_contosoUniversityEntities).Get_Enrollment_ByDate();
        }
    }
}
