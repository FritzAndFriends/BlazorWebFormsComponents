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
    public partial class Courses : WebFormsPageBase
    {
        private AutoCompleteExtender AutoCompleteExtender1 = default!;
        private DropDownList<object> drpDepartments = default!;
        private DetailsView<object> dtlCourses = default!;
        private GridView<object> grvCourses = default!;
        private TextBox txtCourse = default!;
        private List<object>? _coursesDataSource;
        private List<object>? _courseDetailsDataSource;

        [SupplyParameterFromForm(FormName = "CoursesForm", Name = "drpDepartments")]
        public string? PostedDepartment { get; set; }

        [SupplyParameterFromForm(FormName = "CoursesForm", Name = "txtCourse")]
        public string? PostedCourseName { get; set; }

        [SupplyParameterFromForm(FormName = "CoursesForm", Name = "__action")]
        public string? PostedAction { get; set; }

        [Inject]
        protected ContosoUniversityEntities _contosoUniversityEntities { get; set; } = default!;

        private Courses_Logic coursLogic = default!;
        private List<ListItem> _departmentItems = new();

        private ListItemCollection DepartmentStaticItems
        {
            get
            {
                var items = new ListItemCollection();
                items.AddRange(_departmentItems);
                return items;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            coursLogic = new Courses_Logic(_contosoUniversityEntities);

            foreach (var dep in _contosoUniversityEntities.Departments)
            {
                _departmentItems.Add(new ListItem(dep.DepartmentName));
            }

            if (PostedAction == "Search Courses" && !string.IsNullOrWhiteSpace(PostedDepartment))
            {
                _coursesDataSource = coursLogic.GetCourses(PostedDepartment).Cast<object>().ToList();
            }
            else if (PostedAction == "Search" && !string.IsNullOrWhiteSpace(PostedCourseName))
            {
                _courseDetailsDataSource = coursLogic.GetCourse(PostedCourseName).Cast<object>().ToList();
            }
        }

        public static List<string> GetList(string prefixText, int count)
        {
            List<string> name = new List<string>();

            string query = "select CourseName from dbo.[Courses] where CourseName like @SearchText + '%'";
            string connectionStr = ConfigurationManager.ConnectionStrings["ContosoUniversity"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    SqlParameter param = new SqlParameter();
                    param.Direction = ParameterDirection.Input;
                    param.DbType = DbType.String;
                    param.ParameterName = "@SearchText";
                    param.Value = prefixText;

                    cmd.Parameters.Add(param);

                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            name.Add(dr["CourseName"].ToString());
                        }

                        dr.Close();
                    }
                }

                con.Close();
            }
            return name;
        }

        protected void grvCourses_PageIndexChanging(PageChangedEventArgs e)
        {
            this.grvCourses.PageIndex = e.NewPageIndex;
            if (!string.IsNullOrWhiteSpace(PostedDepartment))
            {
                _coursesDataSource = coursLogic.GetCourses(PostedDepartment).Cast<object>().ToList();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
            }
        }
    }
}
