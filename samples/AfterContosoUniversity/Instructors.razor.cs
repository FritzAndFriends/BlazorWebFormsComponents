using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Components;
using BlazorWebFormsComponents;
using ContosoUniversity.Models;
using ContosoUniversity.BLL;

namespace ContosoUniversity
{
    public partial class Instructors
    {
        [Inject] private Instructors_Logic instructorsLogic { get; set; } = default!;

        private List<Instructor> _instructors = new();
        private string _sortDirection = "desc";

        protected override void OnInitialized()
        {
            _instructors = instructorsLogic.getInstructors();
        }

        private void grvInstructors_Sorting(GridViewSortEventArgs e)
        {
            _instructors = instructorsLogic.GetSortedInstrucors(e.SortExpression, _sortDirection);
            ChangeSortDirection();
        }

        private void ChangeSortDirection()
        {
            _sortDirection = _sortDirection == "asc" ? "desc" : "asc";
        }
    }
}
