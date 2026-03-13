using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using ContosoUniversity.Models;
using ContosoUniversity.BLL;
using ContosoUniversity.Bll;

namespace ContosoUniversity
{
    public partial class Courses
    {
        [Inject] private Courses_Logic coursLogic { get; set; } = default!;
        [Inject] private StudentsListLogic studLogic { get; set; } = default!;

        private List<string> _departments = new();
        private string _selectedDepartment = "";
        private List<Cours> _courses = new();
        private string _courseSearchText = "";
        private List<Cours> _courseDetails = new();

        protected override void OnInitialized()
        {
            _departments = studLogic.GetDepartmentNames();
        }

        private void btnSearchCourse_Click()
        {
            if (!string.IsNullOrEmpty(_selectedDepartment))
            {
                _courses = coursLogic.GetCourses(_selectedDepartment);
            }
        }

        private void search_Click()
        {
            if (!string.IsNullOrEmpty(_courseSearchText))
            {
                _courseDetails = coursLogic.GetCourse(_courseSearchText);
                _courseSearchText = "";
            }
        }
    }
}
