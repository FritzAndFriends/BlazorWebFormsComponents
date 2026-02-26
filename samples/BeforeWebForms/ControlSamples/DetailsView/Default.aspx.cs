using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BeforeWebForms.ControlSamples.DetailsView
{
	public partial class Default : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				var products = GetProducts();
				AutoRowsDetailsView.DataSource = products;
				AutoRowsDetailsView.DataBind();

				ExplicitFieldsDetailsView.DataSource = products;
				ExplicitFieldsDetailsView.DataBind();
			}
		}

		protected void AutoRowsDetailsView_PageIndexChanging(object sender, DetailsViewPageEventArgs e)
		{
			AutoRowsDetailsView.PageIndex = e.NewPageIndex;
			AutoRowsDetailsView.DataSource = GetProducts();
			AutoRowsDetailsView.DataBind();
		}

		protected void ExplicitFieldsDetailsView_PageIndexChanging(object sender, DetailsViewPageEventArgs e)
		{
			ExplicitFieldsDetailsView.PageIndex = e.NewPageIndex;
			ExplicitFieldsDetailsView.DataSource = GetProducts();
			ExplicitFieldsDetailsView.DataBind();
		}

		private List<Product> GetProducts()
		{
			return new List<Product>
			{
				new Product { Id = 1, Name = "Widget", Price = 9.99m, Category = "Tools", InStock = true },
				new Product { Id = 2, Name = "Gadget", Price = 19.99m, Category = "Electronics", InStock = true },
				new Product { Id = 3, Name = "Sprocket", Price = 4.50m, Category = "Hardware", InStock = false },
				new Product { Id = 4, Name = "Gizmo", Price = 29.99m, Category = "Electronics", InStock = true },
				new Product { Id = 5, Name = "Doohickey", Price = 14.75m, Category = "Tools", InStock = true },
				new Product { Id = 6, Name = "Thingamajig", Price = 7.25m, Category = "Hardware", InStock = false },
				new Product { Id = 7, Name = "Whatchamacallit", Price = 22.00m, Category = "Electronics", InStock = true },
				new Product { Id = 8, Name = "Contraption", Price = 35.50m, Category = "Tools", InStock = true },
				new Product { Id = 9, Name = "Apparatus", Price = 42.99m, Category = "Hardware", InStock = false },
				new Product { Id = 10, Name = "Mechanism", Price = 18.50m, Category = "Electronics", InStock = true }
			};
		}
	}

	public class Product
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public decimal Price { get; set; }
		public string Category { get; set; }
		public bool InStock { get; set; }
	}
}
