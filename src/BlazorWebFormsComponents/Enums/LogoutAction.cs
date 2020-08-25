using System;

namespace BlazorWebFormsComponents.Enums
{
	public abstract class LogoutAction
	{

		public static RefreshLogoutAction Refresh { get; } = new RefreshLogoutAction();

		public static RedirectLogoutAction Redirect { get; } = new RedirectLogoutAction();

		public static LogoutAction RedirectToLoginPage => throw new NotSupportedException("RedirectToLoginPage is not supported in Blazor.");

	}

	public class RefreshLogoutAction : LogoutAction { }

	public class RedirectLogoutAction : LogoutAction { }

}
