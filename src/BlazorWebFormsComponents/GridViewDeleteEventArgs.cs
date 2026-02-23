using System;

namespace BlazorWebFormsComponents
{
	public class GridViewDeleteEventArgs : EventArgs
	{
		public int RowIndex { get; }
		public bool Cancel { get; set; }

		public GridViewDeleteEventArgs(int rowIndex)
		{
			RowIndex = rowIndex;
		}
	}
}
