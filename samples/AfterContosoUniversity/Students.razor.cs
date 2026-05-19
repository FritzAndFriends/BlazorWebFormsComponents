using System;
using System.Collections.Generic;
using System.Linq;
using ConfigurationManager = BlazorWebFormsComponents.ConfigurationManager;
using ContosoUniversity.Models;
using ContosoUniversity.BLL;
using System.Data.SqlClient;
using System.Data;
 
using Microsoft.AspNetCore.Components;
using BlazorAjaxToolkitComponents;
namespace ContosoUniversity
{
    public partial class Students : WebFormsPageBase
    {
    // TODO(bwfc-general): ClientScript calls preserved — works via WebFormsPageBase (no injection needed). ScriptManagerShim may need @inject ScriptManagerShim ScriptManager for non-page classes.

    // --- Request.Form Migration ---
    // TODO(bwfc-form): Request.Form calls work automatically via RequestShim on WebFormsPageBase.
    // For interactive mode, wrap your form in <WebFormsForm OnSubmit="SetRequestFormData">.
    // Form keys found: key
    // For non-page classes, inject RequestShim via DI.
 
    private AutoCompleteExtender AutoCompleteExtender1 = default!;
    private Button btnSearch = default!;
    private DropDownList<object> dropListCourses = default!;
    private GridView<object> grv = default!;
    private ScriptManager ScriptManager2 = default!;
    private DetailsView<object> studentData = default!;
    private object _studentData_DataSource = null!;
    private TextBox txtSearch = default!;
    private readonly ListItemCollection _courseItems = new();
    private List<object> _gridData = new();
    // --- ConfigurationManager Migration ---
    // TODO(bwfc-config): ConfigurationManager calls work via BWFC shim.
    // Ensure app.UseConfigurationManagerShim() is called in Program.cs.
    // ConnectionString names found: ContosoUniversity
    // Add these to appsettings.json under "ConnectionStrings" section.
 
    [Inject]
    protected ContosoUniversityEntities _contosoUniversityEntities { get; set; } = default!;

    [SupplyParameterFromForm(FormName = "StudentsForm", Name = "txtFirstName")]
    public string? PostedFirstName { get; set; }

    [SupplyParameterFromForm(FormName = "StudentsForm", Name = "txtLastName")]
    public string? PostedLastName { get; set; }

    [SupplyParameterFromForm(FormName = "StudentsForm", Name = "txtBirthDate")]
    public string? PostedBirthDate { get; set; }

    [SupplyParameterFromForm(FormName = "StudentsForm", Name = "txtEmail")]
    public string? PostedEmail { get; set; }

    [SupplyParameterFromForm(FormName = "StudentsForm", Name = "dropListCourses")]
    public string? PostedCourse { get; set; }

    [SupplyParameterFromForm(FormName = "StudentsForm", Name = "__action")]
    public string? PostedAction { get; set; }

    [SupplyParameterFromForm(FormName = "StudentsForm", Name = "__delete")]
    public string? PostedDeleteId { get; set; }
 
        private StudentsListLogic studLogic;
          
        protected override async Task OnInitializedAsync()
        {
            // TODO(bwfc-lifecycle): Review lifecycle conversion — verify async behavior
            await base.OnInitializedAsync();
 
            studLogic = new StudentsListLogic(_contosoUniversityEntities);
 
                        // BWFC: IsPostBack guard unwrapped — Blazor re-renders on every state change
            _courseItems.Clear();
            foreach (var course in _contosoUniversityEntities.Courses)
                        {
                            _courseItems.Add(new ListItem(course.CourseName));
                        }

            if (PostedAction == "New Enrollment")
            {
                InsertPostedEntry();
            }
            else if (PostedAction == "Clear")
            {
                PostedFirstName = string.Empty;
                PostedLastName = string.Empty;
                PostedBirthDate = string.Empty;
                PostedEmail = string.Empty;
                PostedCourse = string.Empty;
            }
            else if (int.TryParse(PostedDeleteId, out var deleteId))
            {
                studLogic.DeleteStudent(deleteId);
            }
 
            _gridData = grv_GetData().ToList();
            _studentData_DataSource = null;
        }
 
        #region Filling Enrollments table
        // The return type can be changed to IEnumerable, however to support
        // paging and sorting, the following parameters must be added:
        //     int maximumRows
        //     int startRowIndex
        //     out int totalRowCount
        //     string sortByExpression
        public IQueryable<Object> grv_GetData()
        {
            IQueryable<Object> list = studLogic.GetJoinedTableData().AsQueryable();              
            return list;
        }
        #endregion
 
        #region Updating Row Data
        protected void grv_RowUpdating(GridViewUpdateEventArgs e)
        {
            int id, counter = 0;
            string name = string.Empty;
            string email = string.Empty;
 
            id = int.Parse(grv.Rows[e.RowIndex].Cells[1].Text);
 
            foreach (string val in e.NewValues.Values)
            {
                if (counter == 0)
                {
                    name = val;
                    counter++;
                }
                else email = val;
            }
 
            studLogic.UpdateStudentData(id, name, email);
        }
        #endregion
 
        #region Delete Row
        // The id parameter name should match the DataKeyNames value set on the control
        public void grv_DeleteItem(int id)
        {
            studLogic.DeleteStudent(id);
        }
        #endregion
 
        #region Insert Button
        protected void btnInsert_Click(EventArgs e)
        {
            InsertPostedEntry();
        }

        private void InsertPostedEntry()
        {
            if (string.IsNullOrWhiteSpace(PostedFirstName) ||
                string.IsNullOrWhiteSpace(PostedLastName) ||
                string.IsNullOrWhiteSpace(PostedBirthDate) ||
                string.IsNullOrWhiteSpace(PostedCourse))
            {
                return;
            }

            if (!DateTime.TryParse(PostedBirthDate, out var birth))
            {
                throw new Exception("Wrong Date Format !!!");
            }

            studLogic.InsertNewEntry(PostedFirstName, PostedLastName, birth, PostedCourse, PostedEmail ?? string.Empty);
        }
        #endregion
 
        #region Clear Button
        protected void btnClear_Click(EventArgs e)
        {
            PostedFirstName = string.Empty;
            PostedLastName = string.Empty;
            PostedBirthDate = string.Empty;
            PostedEmail = string.Empty;
            PostedCourse = string.Empty;
        }
        #endregion
 
        #region AutoComplete WebService
        // TODO(bwfc-webmethod): Migrate legacy static WebMethod endpoint to a Razor component callback or Minimal API.
        // Legacy [WebMethod] attribute removed for Blazor migration.
        // Legacy [ScriptMethod] attribute removed for Blazor migration.
        public static List<string> GetCompletionList(string prefixText, int count)  //This Service must be static in order to work however documentation
        {                                                                            // says nothing about that !!!
            List<string> students = new List<string>();
            string conString = ConfigurationManager.ConnectionStrings["ContosoUniversity"].ConnectionString;
            string fullName;
 
            string sqlQuery = "SELECT FirstName,LastName FROM dbo.Students WHERE FirstName LIKE @SearchText + '%'";
 
            using (SqlConnection con = new SqlConnection(conString))
            {
                using (SqlCommand cmd = new SqlCommand(sqlQuery, con))
                {
                    SqlParameter param = new SqlParameter();
                    param.ParameterName = "@SearchText";
                    param.Value = prefixText;
                    param.SqlDbType = SqlDbType.NVarChar;
                    param.Size = 30;
                    param.Direction = ParameterDirection.Input;
 
                    cmd.Parameters.Add(param);
 
                    con.Open();
 
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            fullName = string.Format("{0} {1}", dr["FirstName"].ToString(), dr["LastName"].ToString());
                            students.Add(fullName);
                        }
 
                        dr.Close();
                    }
 
                    con.Close();
                }
            }
 
            return students;
        }
        #endregion
 
        #region Search Button
        protected void btnSearch_Click(EventArgs e)
        {
            this.studentData.DataSource = studLogic.GetStudents(this.txtSearch.Text);
            this.txtSearch.Text = string.Empty;
        }
        #endregion
 
        //Must Bind Data to GridView manually because of lack of DataSourceID or explicitly set InsertMethod !!!
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // TODO(bwfc-lifecycle): Review lifecycle conversion — verify async behavior
            if (firstRender)
            {
            }
        }
 
        // The id parameter name should match the DataKeyNames value set on the control
        public void grv_UpdateItem(int id)
        {
           
        }  
    }
}
