using SharedSampleObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BeforeWebForms2.ControlSamples.GridView
{
  public partial class TemplateFields : System.Web.UI.Page
  {

		private static bool PriceVisibility { get; set; } = false;
		protected void Page_Load(object sender, EventArgs e)
	{
			TemplateFieldGridView.DataSource = Widget.SimpleWidgetList;
			TemplateFieldGridView.DataBind();
		}

		protected void Unnamed_Click(object sender, EventArgs e)
		{
			foreach(GridViewRow row in TemplateFieldGridView.Rows)
			{
				var label = row.FindControl("lblPrice") as Label;
				label.Visible = !PriceVisibility;
			}
			PriceVisibility = !PriceVisibility;
		}
	}
}

