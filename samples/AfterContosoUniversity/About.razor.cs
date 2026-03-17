using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using ContosoUniversity.Bll;

namespace ContosoUniversity
{
    public partial class About
    {
        [Inject] private Enrollmet_Logic enrollmetLogic { get; set; } = default!;

        private List<KeyValuePair<string, int>> _enrollmentStats = new();

        protected override void OnInitialized()
        {
            _enrollmentStats = enrollmetLogic.Get_Enrollment_ByDate()
                .Select(kvp => new KeyValuePair<string, int>(kvp.Key, kvp.Value))
                .ToList();
        }
    }
}
