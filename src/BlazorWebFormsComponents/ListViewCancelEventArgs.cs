using System;
using BlazorWebFormsComponents.Enums;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for the ItemCanceling event of the ListView control.
	/// </summary>
	public class ListViewCancelEventArgs : EventArgs
	{
		public ListViewCancelEventArgs(int itemIndex, ListViewCancelMode cancelMode)
		{
			ItemIndex = itemIndex;
			CancelMode = cancelMode;
		}

		/// <summary>
		/// Gets the index of the item that contains the Cancel button that raised the event.
		/// </summary>
		public int ItemIndex { get; }

		/// <summary>
		/// Gets the data-entry mode that the ListView was in when the Cancel button was clicked.
		/// </summary>
		public ListViewCancelMode CancelMode { get; }

		/// <summary>
		/// Gets or sets a value indicating whether the event should be cancelled.
		/// </summary>
		public bool Cancel { get; set; }
	}
}
