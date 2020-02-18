using SharedSampleObjects.Models;
using System;
using System.Linq;

namespace BeforeWebForms.ControlSamples.ListView
{
	public partial class ModelBinding : System.Web.UI.Page
	{
		protected void Page_Load(object sender, EventArgs e)
		{

		}

		// The return type can be changed to IEnumerable, however to support
		// paging and sorting, the following parameters must be added:
		//     int maximumRows
		//     int startRowIndex
		//     out int totalRowCount
		//     string sortByExpression
		public IQueryable<Widget> simpleListView_GetData()
		{
			return Widget.SimpleWidgetList.AsQueryable();
		}
	}
}
