using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BeforeWebForms.ControlSamples.Chart
{
	public partial class LineChart : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				// Add data points for Actual Sales
				Chart1.Series["ActualSales"].Points.AddXY("Q1", 45);
				Chart1.Series["ActualSales"].Points.AddXY("Q2", 52);
				Chart1.Series["ActualSales"].Points.AddXY("Q3", 58);
				Chart1.Series["ActualSales"].Points.AddXY("Q4", 65);

				// Add data points for Target Sales
				Chart1.Series["TargetSales"].Points.AddXY("Q1", 50);
				Chart1.Series["TargetSales"].Points.AddXY("Q2", 55);
				Chart1.Series["TargetSales"].Points.AddXY("Q3", 60);
				Chart1.Series["TargetSales"].Points.AddXY("Q4", 70);

				// Enable point labels to show values
				Chart1.Series["ActualSales"].IsValueShownAsLabel = true;
				Chart1.Series["TargetSales"].IsValueShownAsLabel = true;
			}
		}
	}
}
