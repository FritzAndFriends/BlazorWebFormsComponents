using SharedSampleObjects.Models;
using System;
using System.Linq;

namespace BeforeWebForms.ControlSamples.ListView
{
	public partial class Grouping : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			simpleListView.DataSource = Widget.Widgets(7);
			simpleListView.DataBind();
		}
	}
}
