// =============================================================================
// TODO(bwfc-general): This code-behind was copied from Web Forms and needs manual migration.
//
// Common transforms needed (use the BWFC Copilot skill for assistance):
//   TODO(bwfc-lifecycle): Page_Load / Page_Init → OnInitializedAsync / OnParametersSetAsync
//   TODO(bwfc-lifecycle): Page_PreRender → OnAfterRenderAsync
//   TODO(bwfc-ispostback): IsPostBack checks → remove or convert to state logic
//   TODO(bwfc-viewstate): ViewState usage → component [Parameter] or private fields
//   TODO(bwfc-session-state): Session/Cache access → auto-wired on WebFormsPageBase via SessionShim/CacheShim
//   TODO(bwfc-navigation): Response.Redirect → auto-wired on WebFormsPageBase via ResponseShim
//   TODO(bwfc-form): Request.Form["key"] → auto-wired on WebFormsPageBase via FormShim (use <WebFormsForm> for interactive mode)
//   TODO(bwfc-server): Server.MapPath/HtmlEncode → auto-wired on WebFormsPageBase via ServerShim
//   TODO(bwfc-config): ConfigurationManager.AppSettings → BWFC shim (call app.UseConfigurationManagerShim() in Program.cs)
//   TODO(bwfc-general): ClientScript.RegisterStartupScript → auto-wired on WebFormsPageBase via ClientScriptShim
//   TODO(bwfc-general): Event handlers (Button_Click, etc.) → convert to Blazor event callbacks
//   TODO(bwfc-datasource): Data binding (DataBind, DataSource) → component parameters or OnInitialized
//   TODO(bwfc-general): ScriptManager code-behind references → use ScriptManagerShim via ScriptManager.GetCurrent(this)
//   TODO(bwfc-general): UpdatePanel markup preserved by BWFC (ContentTemplate supported) — remove only code-behind API calls
//   TODO(bwfc-general): User controls → Blazor component references
// =============================================================================
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using BlazorAjaxToolkitComponents;
using BlazorWebFormsComponents;
using ConfigurationManager = BlazorWebFormsComponents.ConfigurationManager;
using ContosoUniversity.BLL;
using ContosoUniversity.Models;
using Microsoft.AspNetCore.Components;

namespace ContosoUniversity
{
    public partial class Students : WebFormsPageBase
    {
        private AutoCompleteExtender AutoCompleteExtender1 = default!;
        private DropDownList<object> dropListCourses = default!;
        private GridView<object> grv = default!;
        private ScriptManager ScriptManager2 = default!;
        private DetailsView<object> studentData = default!;
        private List<object>? _studentData_DataSource;
        private List<object> _gridData = new();
        private Table tabAddStud = default!;
        private TextBox txtBirthDate = default!;
        private TextBox txtEmail = default!;
        private TextBox txtFirstName = default!;
        private TextBox txtLastName = default!;
        private TextBox txtSearch = default!;

        [SupplyParameterFromForm(FormName = "StudentsForm", Name = "tabAddStud$txtFirstName")]
        public string? PostedFirstName { get; set; }

        [SupplyParameterFromForm(FormName = "StudentsForm", Name = "tabAddStud$txtLastName")]
        public string? PostedLastName { get; set; }

        [SupplyParameterFromForm(FormName = "StudentsForm", Name = "tabAddStud$txtBirthDate")]
        public string? PostedBirthDate { get; set; }

        [SupplyParameterFromForm(FormName = "StudentsForm", Name = "tabAddStud$txtEmail")]
        public string? PostedEmail { get; set; }

        [SupplyParameterFromForm(FormName = "StudentsForm", Name = "tabAddStud$dropListCourses")]
        public string? PostedCourse { get; set; }

        [SupplyParameterFromForm(FormName = "StudentsForm", Name = "__action")]
        public string? PostedAction { get; set; }

        [SupplyParameterFromForm(FormName = "StudentsForm", Name = "__delete")]
        public string? PostedDeleteId { get; set; }

        [SupplyParameterFromForm(FormName = "StudentsForm", Name = "txtSearch")]
        public string? PostedSearchText { get; set; }

        [Inject]
        protected ContosoUniversityEntities _contosoUniversityEntities { get; set; } = default!;

        private StudentsListLogic studLogic = default!;
        private List<ListItem> _courseItems = new();

        private ListItemCollection CourseStaticItems
        {
            get
            {
                var items = new ListItemCollection();
                items.AddRange(_courseItems);
                return items;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            studLogic = new StudentsListLogic(_contosoUniversityEntities);

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
            else if (PostedAction == "Show Student Info")
            {
                if (!string.IsNullOrWhiteSpace(PostedSearchText))
                {
                    _studentData_DataSource = studLogic.GetStudents(PostedSearchText);
                }
            }
            else if (int.TryParse(PostedDeleteId, out var deleteId))
            {
                studLogic.DeleteStudent(deleteId);
            }

            _gridData = grv_GetData().ToList();
        }

        public IQueryable<object> grv_GetData()
        {
            return studLogic.GetJoinedTableData().AsQueryable();
        }

        protected void grv_RowUpdating(GridViewUpdateEventArgs e)
        {
            return;
        }

        public void grv_DeleteItem(int id)
        {
            studLogic.DeleteStudent(id);
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
                return;
            }

            studLogic.InsertNewEntry(PostedFirstName, PostedLastName, birth, PostedCourse, PostedEmail ?? string.Empty);
        }

        // TODO(bwfc-webmethod): Migrate legacy static WebMethod endpoint to a Razor component callback or Minimal API.
        // Legacy [WebMethod] attribute removed for Blazor migration.
        // Legacy [ScriptMethod] attribute removed for Blazor migration.
        public static List<string> GetCompletionList(string prefixText, int count)
        {
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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
            }
        }

        public void grv_UpdateItem(int id)
        {
        }
    }
}
