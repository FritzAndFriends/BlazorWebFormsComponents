using Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebProject
{
	public partial class _Default : Page
	{

		public readonly IEnumerable<CartLineItem> LineItems = new CartLineItem[] {
			new CartLineItem { LineItemID=1, ProductName="Product 1", Quantity=2, UnitPrice=3.99m },
			new CartLineItem { LineItemID=2, ProductName="Product 2", Quantity=4, UnitPrice=5.99m },
			new CartLineItem { LineItemID=3, ProductName="Product 3", Quantity=6, UnitPrice=2.99m },
			new CartLineItem { LineItemID=4, ProductName="Product 4", Quantity=8, UnitPrice=9.99m },
		};


		protected void Page_Load(object sender, EventArgs e)
		{

			LineItemList.DataSource = LineItems;
			LineItemList.DataBind();

		}

		protected void CalculateTax_Click(object sender, EventArgs e)
		{

			TaxValue.InnerText = CartCalculations.CalculateTax(decimal.Parse(CurrentPrice.Value)).ToString("c");

		}

		protected void CalculateTotals_Click(object sender, EventArgs e)
		{

			var totals = CartCalculations.TotalItems(LineItems);
			TotalPrice.InnerText = totals.totalPrice.ToString("c");
			CountItems.InnerText = totals.count.ToString();

		}
	}
}
