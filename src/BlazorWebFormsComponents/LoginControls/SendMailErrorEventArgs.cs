using System;

namespace BlazorWebFormsComponents.LoginControls
{
	public class SendMailErrorEventArgs : EventArgs
	{

		public string ErrorMessage { get; set; }

		/// <summary>
		/// The component that raised this event
		/// </summary>
		public object Sender { get; set; }

	}
}
