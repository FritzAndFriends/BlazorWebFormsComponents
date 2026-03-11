using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

using ContosoUniversity.Models;

namespace ContosoUniversity.BLL
{
    public class Instructors_Logic
    {
        #region Get Instructors List
        public List<Instructor> getInstructors()
        {
            var instructors = (from instructor in new ContosoUniversityEntities().Instructors
                               select instructor).ToList<Instructor>();

            return instructors;
        }
        #endregion

        #region Get Sorted Instructors List
        public List<Instructor> GetSortedInstrucors(string expression,string direction)
        {           
            List<Instructor> list = new List<Instructor>();
            string query = "select * from dbo.[Instructors]";
            string connectionStr = ConfigurationManager.ConnectionStrings["ContosoUniversity"].ConnectionString;

            if (!String.IsNullOrEmpty(expression))
            {
                query += " order by " + expression + " " + direction;
            }

            using (SqlConnection con = new SqlConnection(connectionStr))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            Instructor instr = new Instructor();

                            instr.InstructorID = Convert.ToInt32(dr["InstructorID"]);
                            instr.FirstName = dr["FirstName"].ToString();
                            instr.LastName = dr["LastName"].ToString();
                            instr.BirthDate = DateTime.Parse(dr["BirthDate"].ToString());
                            instr.Email = dr["Email"].ToString();

                            list.Add(instr);
                        }
                        dr.Close();
                    }                   
                }
                con.Close();
            }
            return list;
        }
        #endregion
    }
}