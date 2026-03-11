using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using ContosoUniversity.Models;
using ContosoUniversity.Bll;


namespace ContosoUniversity
{
    public partial class About : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        // The return type can be changed to IEnumerable, however to support
        // paging and sorting, the following parameters must be added:
        //     int maximumRows
        //     int startRowIndex
        //     out int totalRowCount
        //     string sortByExpression
        public Dictionary<string, int> EnrollmentsStat_GetData()
        {
            return new Enrollmet_Logic().Get_Enrollment_ByDate();
        }
    }
}