using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Drawing;

namespace BeforeWebForms.ControlSamples.Chart
{
	public partial class PieChart : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				// Add data points to the pie chart
				var point1 = Chart1.Series["MarketShare"].Points.AddXY("Product A", 35);
				var point2 = Chart1.Series["MarketShare"].Points.AddXY("Product B", 25);
				var point3 = Chart1.Series["MarketShare"].Points.AddXY("Product C", 20);
				var point4 = Chart1.Series["MarketShare"].Points.AddXY("Product D", 15);
				var point5 = Chart1.Series["MarketShare"].Points.AddXY("Others", 5);

				// Set custom colors for each slice
				Chart1.Series["MarketShare"].Points[0].Color = Color.FromArgb(70, 130, 180); // SteelBlue
				Chart1.Series["MarketShare"].Points[1].Color = Color.FromArgb(60, 179, 113); // MediumSeaGreen
				Chart1.Series["MarketShare"].Points[2].Color = Color.FromArgb(255, 165, 0);  // Orange
				Chart1.Series["MarketShare"].Points[3].Color = Color.FromArgb(220, 20, 60);  // Crimson
				Chart1.Series["MarketShare"].Points[4].Color = Color.FromArgb(169, 169, 169); // DarkGray

				// Show percentage labels
				Chart1.Series["MarketShare"].Label = "#PERCENT{P1}";
			}
		}
	}
}
