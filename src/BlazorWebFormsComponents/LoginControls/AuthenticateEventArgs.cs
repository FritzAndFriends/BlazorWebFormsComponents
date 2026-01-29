using System;

namespace BlazorWebFormsComponents.LoginControls
{
	public class AuthenticateEventArgs : EventArgs
	{

		public bool Authenticated { get; set; }

		/// <summary>
		/// The component that raised this event
		/// </summary>
		public object Sender { get; set; }

	}
}
