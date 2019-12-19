﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BeforeWebForms.ControlSamples.Repeater
{
	public partial class Default : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			repeaterControl.DataSource = SharedSampleObjects.Models.Widget.SimpleWidgetList;
			repeaterControl.DataBind();
		}
	}
}