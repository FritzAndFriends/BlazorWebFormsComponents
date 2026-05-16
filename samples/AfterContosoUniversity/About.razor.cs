using System;
using System.Collections.Generic;
using System.Linq;
using ContosoUniversity.Models;
using ContosoUniversity.Bll;
using Microsoft.AspNetCore.Components;


namespace ContosoUniversity
{
    public partial class About : WebFormsPageBase
    {
    [Inject] private Enrollmet_Logic _enrollmetLogic { get; set; } = default!;

    private GridView<object> EnrollmentsStat = default!;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            
        }

        public Dictionary<string, int> EnrollmentsStat_GetData()
        {
            return _enrollmetLogic.Get_Enrollment_ByDate();
        }
    }
}