using SharedSampleObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BeforeWebForms.ControlSamples.GridView
{

	public partial class Default : System.Web.UI.Page
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
		public List<Customer> GetCustomers()
		{

			return Customer.GetCustomers(5,0, "", out _).ToList();

		}
	}
}
