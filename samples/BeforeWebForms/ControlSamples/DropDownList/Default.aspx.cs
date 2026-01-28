using System;
using System.Collections.Generic;
using System.Web.UI;

namespace BeforeWebForms.ControlSamples.DropDownList
{
	public partial class Default : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				ddlDataBound.DataSource = new[]
				{
					new { Id = "1", Name = "First Item" },
					new { Id = "2", Name = "Second Item" },
					new { Id = "3", Name = "Third Item" }
				};
				ddlDataBound.DataBind();
			}
		}
	}
}
