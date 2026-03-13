using Microsoft.AspNetCore.Components;
using ContosoUniversity.Bll;

namespace ContosoUniversity
{
    public partial class About
    {
        [Inject] private Enrollmet_Logic EnrollmentLogic { get; set; }

        private List<KeyValuePair<string, int>> _enrollmentData;

        protected override async Task OnInitializedAsync()
        {
            var data = EnrollmentLogic.Get_Enrollment_ByDate();
            _enrollmentData = data.Select(kvp => new KeyValuePair<string, int>(kvp.Key, kvp.Value)).ToList();
        }
    }
}
