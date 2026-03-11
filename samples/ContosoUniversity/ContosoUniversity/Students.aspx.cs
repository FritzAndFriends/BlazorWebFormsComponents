using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Configuration;
using ContosoUniversity.Models;
using ContosoUniversity.Bll;
using System.Data.SqlClient;
using System.Data;

namespace ContosoUniversity
{
    public partial class Students : System.Web.UI.Page
    {
        private StudentsListLogic studLogic;
          
        protected void Page_Load(object sender, EventArgs e)
        {
            studLogic = new StudentsListLogic();

            if (!IsPostBack)
            {
                foreach (var course in new ContosoUniversityEntities().Courses)
                {
                    this.dropListCourses.Items.Add(course.CourseName);
                }
            }

            this.studentData.DataSource = null;
            this.studentData.DataBind();    
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
        protected void grv_RowUpdating(object sender, GridViewUpdateEventArgs e)
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
        protected void btnInsert_Click(object sender, EventArgs e)
        {
            DateTime birth;

            try
            {
                birth = DateTime.Parse(txtBirthDate.Text);
            }
            catch
            {
                throw new Exception("Wrong Date Format !!!");
            }

            studLogic.InsertNewEntry(txtFirstName.Text,txtLastName.Text,birth,dropListCourses.SelectedItem.ToString(),txtEmail.Text);
           
        }
        #endregion

        #region Clear Button
        protected void btnClear_Click(object sender, EventArgs e)
        {
            this.txtFirstName.Text = string.Empty;

            foreach (TableRow row in this.tabAddStud.Rows)
            {
                if (row.Cells[1].Controls[0] is TextBox)
                {
                    (row.Cells[1].Controls[0] as TextBox).Text = string.Empty;
                }
                else if (row.Cells[1].Controls[0] is DropDownList)
                {
                    (row.Cells[1].Controls[0] as DropDownList).ClearSelection();
                }              
            }
        }
        #endregion

        #region AutoComplete WebService
        [System.Web.Services.WebMethod]
        [System.Web.Script.Services.ScriptMethod]
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
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.studentData.DataSource = studLogic.GetStudents(this.txtSearch.Text);
            this.studentData.DataBind();
            this.txtSearch.Text = string.Empty;
        }
        #endregion

        //Must Bind Data to GridView manually because of lack of DataSourceID or explicitly set InsertMethod !!!
        protected void Page_PreRenderComplete(object sender, EventArgs e)
        {
            this.grv.DataBind();           
        }

        // The id parameter name should match the DataKeyNames value set on the control
        public void grv_UpdateItem(int id)
        {
           
        }  
    }
}
