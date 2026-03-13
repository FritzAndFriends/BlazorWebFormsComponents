using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Components;
using BlazorWebFormsComponents;
using ContosoUniversity.Models;
using ContosoUniversity.Bll;

namespace ContosoUniversity
{
    public partial class Students
    {
        [Inject] private StudentsListLogic studLogic { get; set; } = default!;

        private List<object> _students = new();
        private List<string> _courseNames = new();
        private string _selectedCourse = "";
        private string _firstName = "";
        private string _lastName = "";
        private string _birthDate = "";
        private string _email = "";
        private string _searchText = "";
        private List<object> _studentDetails = new();

        protected override void OnInitialized()
        {
            _students = studLogic.GetJoinedTableData();
            _courseNames = studLogic.GetCourseNames();
        }

        private void grv_DeleteItem(int id)
        {
            studLogic.DeleteStudent(id);
            _students = studLogic.GetJoinedTableData();
        }

        private void grv_RowUpdating(GridViewUpdateEventArgs e)
        {
            // BWFC GridViewUpdateEventArgs has RowIndex only — full edit support requires manual implementation
        }

        private void btnInsert_Click()
        {
            DateTime birth;
            try
            {
                birth = DateTime.Parse(_birthDate);
            }
            catch
            {
                return;
            }

            studLogic.InsertNewEntry(_firstName, _lastName, birth, _selectedCourse, string.IsNullOrEmpty(_email) ? "Has not specified" : _email);
            _students = studLogic.GetJoinedTableData();
            btnClear_Click();
        }

        private void btnClear_Click()
        {
            _firstName = "";
            _lastName = "";
            _birthDate = "";
            _email = "";
            _selectedCourse = "";
        }

        private void btnSearch_Click()
        {
            if (!string.IsNullOrEmpty(_searchText))
            {
                _studentDetails = studLogic.GetStudents(_searchText);
                _searchText = "";
            }
        }
    }
}
