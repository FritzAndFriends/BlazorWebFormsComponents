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
using System.Linq;
using BlazorWebFormsComponents;
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
    private Button btnClear = default!;
    private Button btnSearch = default!;
    private DropDownList<object> dropListCourses = default!;
    private GridView<object> grv = default!;
    private ScriptManager ScriptManager2 = default!;
    private DetailsView<object> studentData = default!;
    private object _studentData_DataSource = null!;
    private Table tabAddStud = default!;
    private TextBox txtBirthDate = default!;
    private TextBox txtEmail = default!;
    private TextBox txtFirstName = default!;
    private TextBox txtLastName = default!;
    private TextBox txtSearch = default!;
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
        private ListItemCollection _courseItems = new();
          
        protected override async Task OnInitializedAsync()
        {
            // TODO(bwfc-lifecycle): Review lifecycle conversion — verify async behavior
            await base.OnInitializedAsync();

            studLogic = new StudentsListLogic(_contosoUniversityEntities);

                        // BWFC: IsPostBack guard unwrapped — Blazor re-renders on every state change
            foreach (var course in _contosoUniversityEntities.Courses)
                        {
                            _courseItems.Add(new ListItem(course.CourseName));
                        }

            // Handle form POST actions
            if (!string.IsNullOrEmpty(PostedAction) && PostedAction == "New Enrollment")
            {
                HandleInsert();
            }
            else if (!string.IsNullOrEmpty(PostedDeleteId))
            {
                if (int.TryParse(PostedDeleteId, out var deleteId))
                {
                    studLogic.DeleteStudent(deleteId);
                }
            }

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
        private void HandleInsert()
        {
            DateTime birth;

            try
            {
                birth = DateTime.Parse(PostedBirthDate ?? string.Empty);
            }
            catch
            {
                throw new Exception("Wrong Date Format !!!");
            }

            studLogic.InsertNewEntry(PostedFirstName ?? string.Empty, PostedLastName ?? string.Empty, birth, PostedCourse ?? string.Empty, PostedEmail ?? string.Empty);
        }

        protected void btnInsert_Click(EventArgs e)
        {
            HandleInsert();
        }
        #endregion

        #region Clear Button
        protected void btnClear_Click(EventArgs e)
        {
            this.txtFirstName.Text = string.Empty;
            this.txtLastName.Text = string.Empty;
            this.txtBirthDate.Text = string.Empty;
            this.txtEmail.Text = string.Empty;
            this.dropListCourses.SelectedIndex = -1;
            this.dropListCourses.SelectedValue = string.Empty;
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

