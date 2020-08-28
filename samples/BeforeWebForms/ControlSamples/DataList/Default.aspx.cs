using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BeforeWebForms.ControlSamples.DataList
{
	public partial class Default : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			//simpleDataList.DataSource = new SharedSampleObjects.Models.Widget[] { }; // SharedSampleObjects.Models.Widget.SimpleWidgetList;
			simpleDataList.DataSource = SharedSampleObjects.Models.Widget.SimpleWidgetList;
			simpleDataList.DataBind();
		}
	}
}
