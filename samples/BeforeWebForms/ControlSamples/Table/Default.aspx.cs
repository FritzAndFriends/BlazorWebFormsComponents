using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BeforeWebForms.ControlSamples.Table
{
	public partial class Default : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				BuildProgrammaticTable();
			}
		}

		private void BuildProgrammaticTable()
		{
			// Header row
			TableHeaderRow headerRow = new TableHeaderRow();
			headerRow.Cells.Add(new TableHeaderCell { Text = "Month" });
			headerRow.Cells.Add(new TableHeaderCell { Text = "Sales" });
			headerRow.Cells.Add(new TableHeaderCell { Text = "Target" });
			Table3.Rows.Add(headerRow);

			// Data rows
			string[][] data = new string[][]
			{
				new[] { "January", "$1,200", "$1,000" },
				new[] { "February", "$1,500", "$1,200" },
				new[] { "March", "$1,800", "$1,500" },
				new[] { "April", "$2,100", "$1,800" }
			};

			foreach (var row in data)
			{
				TableRow tr = new TableRow();
				foreach (var cell in row)
				{
					tr.Cells.Add(new TableCell { Text = cell });
				}
				Table3.Rows.Add(tr);
			}
		}
	}
}
