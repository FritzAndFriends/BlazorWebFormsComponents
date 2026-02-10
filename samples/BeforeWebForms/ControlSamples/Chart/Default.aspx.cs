using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BeforeWebForms.ControlSamples.Chart
{
	public partial class Default : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				// Add data points to the chart
				Chart1.Series["Sales"].Points.AddXY("January", 12000);
				Chart1.Series["Sales"].Points.AddXY("February", 15000);
				Chart1.Series["Sales"].Points.AddXY("March", 18000);
				Chart1.Series["Sales"].Points.AddXY("April", 16000);
				Chart1.Series["Sales"].Points.AddXY("May", 21000);
				Chart1.Series["Sales"].Points.AddXY("June", 25000);
			}
		}
	}
}
