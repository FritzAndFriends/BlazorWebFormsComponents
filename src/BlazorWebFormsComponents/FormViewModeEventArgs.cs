using BlazorWebFormsComponents.Enums;
using System;

namespace BlazorWebFormsComponents
{
	public class FormViewModeEventArgs : EventArgs
	{

		public bool Cancel { get; set; }

		public bool CancelingEdit { get; set; }

		public FormViewMode NewMode { get; set; }

		/// <summary>
		/// The component that raised this event
		/// </summary>
		public object Sender { get; set; }

	}

}
