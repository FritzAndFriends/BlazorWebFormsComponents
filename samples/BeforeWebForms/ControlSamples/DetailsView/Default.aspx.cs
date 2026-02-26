using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using SharedSampleObjects.Models;

namespace BeforeWebForms.ControlSamples.DetailsView
{
	public partial class Default : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				var products = Product.GetProducts();
				AutoRowsDetailsView.DataSource = products;
				AutoRowsDetailsView.DataBind();

				ExplicitFieldsDetailsView.DataSource = products;
				ExplicitFieldsDetailsView.DataBind();
			}
		}

		protected void AutoRowsDetailsView_PageIndexChanging(object sender, DetailsViewPageEventArgs e)
		{
			AutoRowsDetailsView.PageIndex = e.NewPageIndex;
			AutoRowsDetailsView.DataSource = Product.GetProducts();
			AutoRowsDetailsView.DataBind();
		}

		protected void ExplicitFieldsDetailsView_PageIndexChanging(object sender, DetailsViewPageEventArgs e)
		{
			ExplicitFieldsDetailsView.PageIndex = e.NewPageIndex;
			ExplicitFieldsDetailsView.DataSource = Product.GetProducts();
			ExplicitFieldsDetailsView.DataBind();
		}
	}
}
