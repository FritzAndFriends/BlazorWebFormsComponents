using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

using ContosoUniversity.BLL;
using ContosoUniversity.Models;

namespace ContosoUniversity
{
    public partial class Courses : System.Web.UI.Page
    {
        private Courses_Logic coursLogic;

        protected void Page_Load(object sender, EventArgs e)
        {
            coursLogic = new Courses_Logic();

            if (!IsPostBack)
            {
                foreach (var dep in new ContosoUniversityEntities().Departments)
                {
                    this.drpDepartments.Items.Add(dep.DepartmentName);
                }
            }      
        }

        #region AutoComplete WebService
        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod]
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
        protected void btnSearchCourse_Click(object sender, EventArgs e)
        {
            this.grvCourses.DataSource = coursLogic.GetCourses(this.drpDepartments.SelectedValue);
        }
        #endregion

        #region Search Course By Course Name
        protected void search_Click(object sender, EventArgs e)
        {
            this.dtlCourses.DataSource = coursLogic.GetCourse(this.txtCourse.Text);
            this.txtCourse.Text = string.Empty;
        }
        #endregion

        protected void grvCourses_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            this.grvCourses.PageIndex = e.NewPageIndex;
            btnSearchCourse_Click(null, null);
        }
      
        protected void Page_PreRender(object sender, EventArgs e)
        {
            this.grvCourses.DataBind();
            this.dtlCourses.DataBind();
        }
    }
}