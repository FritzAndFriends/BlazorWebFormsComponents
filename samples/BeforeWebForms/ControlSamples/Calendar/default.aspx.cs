using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BeforeWebForms.ControlSamples.Calendar
{
	public partial class _default : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		protected void CalendarEvents_SelectionChanged(object sender, EventArgs e)
		{
			lblSelected.Text = "Selected: " + CalendarEvents.SelectedDate.ToShortDateString();
		}

		protected void CalendarEvents_VisibleMonthChanged(object sender, System.Web.UI.WebControls.MonthChangedEventArgs e)
		{
			lblMonthChanged.Text = "Month changed to: " + e.NewDate.ToString("MMMM yyyy");
		}
	}
}
