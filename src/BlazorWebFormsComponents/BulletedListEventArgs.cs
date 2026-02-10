using System;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// Provides data for the Click event of a BulletedList control.
	/// </summary>
	public class BulletedListEventArgs : EventArgs
	{
		/// <summary>
		/// Initializes a new instance of the BulletedListEventArgs class.
		/// </summary>
		/// <param name="index">The zero-based index of the clicked item.</param>
		public BulletedListEventArgs(int index)
		{
			Index = index;
		}

		/// <summary>
		/// Gets the zero-based index of the item that was clicked in the BulletedList control.
		/// </summary>
		public int Index { get; }
	}
}
