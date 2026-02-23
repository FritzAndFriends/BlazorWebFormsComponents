using System;

namespace BlazorWebFormsComponents
{
	public class GridViewEditEventArgs : EventArgs
	{
		public int NewEditIndex { get; set; }
		public bool Cancel { get; set; }

		public GridViewEditEventArgs(int newEditIndex)
		{
			NewEditIndex = newEditIndex;
		}
	}
}
