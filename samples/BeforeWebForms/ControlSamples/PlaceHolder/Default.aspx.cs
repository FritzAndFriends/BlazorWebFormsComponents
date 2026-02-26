using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BeforeWebForms.ControlSamples.PlaceHolder
{
	public partial class Default : Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				PlaceHolder1.Controls.Add(new LiteralControl("<p>This content was added programmatically.</p>"));
				PlaceHolder1.Controls.Add(new LiteralControl("<p>PlaceHolder renders no HTML of its own.</p>"));
			}
		}
	}
}
