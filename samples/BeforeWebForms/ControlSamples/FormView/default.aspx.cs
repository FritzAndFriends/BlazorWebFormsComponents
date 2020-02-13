using SharedSampleObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BeforeWebForms.ControlSamples.FormView
{
	public partial class _default : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

			this.WidgetFormView.DataSource = Widget.SimpleWidgetList;
			this.WidgetFormView.DataBind();

		}

		protected void WidgetFormView_PageIndexChanging(object sender, FormViewPageEventArgs e)
		{

		}
	}
}
