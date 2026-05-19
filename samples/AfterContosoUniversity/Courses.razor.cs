using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using ConfigurationManager = BlazorWebFormsComponents.ConfigurationManager;
using ContosoUniversity.BLL;
using ContosoUniversity.Models;

using Microsoft.AspNetCore.Components;
using BlazorAjaxToolkitComponents;
namespace ContosoUniversity
{
    public partial class Courses : WebFormsPageBase
    {
    // TODO(bwfc-general): ClientScript calls preserved — works via WebFormsPageBase (no injection needed). ScriptManagerShim may need @inject ScriptManagerShim ScriptManager for non-page classes.

    // --- Request.Form Migration ---
    // TODO(bwfc-form): Request.Form calls work automatically via RequestShim on WebFormsPageBase.
    // For interactive mode, wrap your form in <WebFormsForm OnSubmit="SetRequestFormData">.
    // Form keys found: key
    // For non-page classes, inject RequestShim via DI.
 
    private AutoCompleteExtender AutoCompleteExtender1 = default!;
    private Button btnSearchCourse = default!;
    private DropDownList<object> drpDepartments = default!;
    private DetailsView<object> dtlCourses = default!;
    private GridView<object> grvCourses = default!;
    private Button search = default!;
    private TextBox txtCourse = default!;
    private readonly ListItemCollection _departmentItems = new();
    // --- ConfigurationManager Migration ---
    // TODO(bwfc-config): ConfigurationManager calls work via BWFC shim.
    // Ensure app.UseConfigurationManagerShim() is called in Program.cs.
    // ConnectionString names found: ContosoUniversity
    // Add these to appsettings.json under "ConnectionStrings" section.

    [Inject]
    protected ContosoUniversityEntities _contosoUniversityEntities { get; set; } = default!;
 
        private Courses_Logic coursLogic;
 
        protected override async Task OnInitializedAsync()
        {
            // TODO(bwfc-lifecycle): Review lifecycle conversion — verify async behavior
            await base.OnInitializedAsync();
 
            coursLogic = new Courses_Logic(_contosoUniversityEntities);
 
                        // BWFC: IsPostBack guard unwrapped — Blazor re-renders on every state change
            _departmentItems.Clear();
            foreach (var dep in _contosoUniversityEntities.Departments)
                        {
                            _departmentItems.Add(new ListItem(dep.DepartmentName));
                        }      
        }
 
        #region AutoComplete WebService
        // TODO(bwfc-webmethod): Migrate legacy static WebMethod endpoint to a Razor component callback or Minimal API.
        // Legacy [WebMethod] attribute removed for Blazor migration.
        // Legacy [ScriptMethod] attribute removed for Blazor migration.
        public static List<string> GetList(string prefixText, int count)  //This Service must be static in order to work however documentation
        {                                                                            // says nothing about that !!!
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
        #endregion
 
        #region Search Course By Department Button
        protected void btnSearchCourse_Click(EventArgs e)
        {
            this.grvCourses.DataSource = coursLogic.GetCourses(this.drpDepartments.SelectedValue);
        }
        #endregion
 
        #region Search Course By Course Name
        protected void search_Click(EventArgs e)
        {
            this.dtlCourses.DataSource = coursLogic.GetCourse(this.txtCourse.Text);
            this.txtCourse.Text = string.Empty;
        }
        #endregion
 
        protected void grvCourses_PageIndexChanging(PageChangedEventArgs e)
        {
            this.grvCourses.PageIndex = e.NewPageIndex;
            btnSearchCourse_Click(EventArgs.Empty);
        }
      
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // TODO(bwfc-lifecycle): Review lifecycle conversion — verify async behavior
            if (firstRender)
            {
            }
        }
    }
}
