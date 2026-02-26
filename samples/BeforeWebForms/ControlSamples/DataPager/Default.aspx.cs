using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;
using SharedSampleObjects.Models;

namespace BeforeWebForms.ControlSamples.DataPager
{
	public partial class Default : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				ProductListView.DataSource = Product.GetProducts();
				ProductListView.DataBind();
			}
		}

		protected void Page_PreRender(object sender, EventArgs e)
		{
			if (IsPostBack)
			{
				ProductListView.DataSource = Product.GetProducts();
				ProductListView.DataBind();
			}
		}
	}
}
