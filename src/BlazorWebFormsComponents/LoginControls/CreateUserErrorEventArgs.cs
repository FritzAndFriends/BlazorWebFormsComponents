using System;

namespace BlazorWebFormsComponents.LoginControls
{
	/// <summary>
	/// Provides data for the CreateUserError event of CreateUserWizard.
	/// </summary>
	public class CreateUserErrorEventArgs : EventArgs
	{
		public string ErrorMessage { get; set; }

		/// <summary>
		/// The component that raised this event.
		/// </summary>
		public object Sender { get; set; }
	}
}
