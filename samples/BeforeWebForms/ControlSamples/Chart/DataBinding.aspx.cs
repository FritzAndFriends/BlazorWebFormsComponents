using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BeforeWebForms.ControlSamples.Chart
{
	public partial class DataBinding : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		/// <summary>
		/// Data source method for ObjectDataSource
		/// Returns a collection of customer sales data
		/// </summary>
		public static List<CustomerSales> GetCustomerSales()
		{
			return new List<CustomerSales>
			{
				new CustomerSales { CompanyName = "Contoso Ltd", TotalSales = 125000 },
				new CustomerSales { CompanyName = "Fabrikam Inc", TotalSales = 95000 },
				new CustomerSales { CompanyName = "Adventure Works", TotalSales = 150000 },
				new CustomerSales { CompanyName = "Northwind Traders", TotalSales = 110000 },
				new CustomerSales { CompanyName = "Wide World Importers", TotalSales = 135000 }
			};
		}
	}

	/// <summary>
	/// Simple data class for customer sales information
	/// </summary>
	public class CustomerSales
	{
		public string CompanyName { get; set; }
		public decimal TotalSales { get; set; }
	}
}
