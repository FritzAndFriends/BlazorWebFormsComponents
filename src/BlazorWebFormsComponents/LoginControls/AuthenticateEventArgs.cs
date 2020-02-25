using System;
using static BlazorWebFormsComponents.LoginControls.Login;

namespace BlazorWebFormsComponents.LoginControls
{
  public class AuthenticateEventArgs : EventArgs
	{
		public bool Authenticated { get; set; }
		public LoginModel loginModel { get; set; }

	}
}
