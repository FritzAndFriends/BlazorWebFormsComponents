using System;

namespace BlazorWebFormsComponents
{
	public class GridViewCancelEditEventArgs : EventArgs
	{
		public int RowIndex { get; }
		public bool Cancel { get; set; }

		public GridViewCancelEditEventArgs(int rowIndex)
		{
			RowIndex = rowIndex;
		}
	}
}
