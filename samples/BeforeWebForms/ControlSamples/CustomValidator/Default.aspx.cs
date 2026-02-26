using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BeforeWebForms.ControlSamples.CustomValidator
{
	public partial class Default : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
		}

		protected void cvEven_ServerValidate(object source, ServerValidateEventArgs args)
		{
			int number;
			if (int.TryParse(args.Value, out number))
			{
				args.IsValid = (number % 2 == 0);
			}
			else
			{
				args.IsValid = false;
			}
		}

		protected void cvAgree_ServerValidate(object source, ServerValidateEventArgs args)
		{
			args.IsValid = chkAgree.Checked;
		}
	}
}
