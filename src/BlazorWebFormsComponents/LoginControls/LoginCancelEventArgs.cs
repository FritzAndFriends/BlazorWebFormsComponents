using System;

namespace BlazorWebFormsComponents.LoginControls
{
	public class LoginCancelEventArgs : EventArgs
	{

		public bool Cancel { get; set; }

		/// <summary>
		/// The component that raised this event
		/// </summary>
		public object Sender { get; set; }

	}
}
