namespace BlazorWebFormsComponents.Test.LoginControls.Login
{
  public class Credential
  {
		public static readonly Credential ValidCredential = new Credential { Username = "admin", Password = "FakePassword" };

		public static readonly Credential InvalidCredential = new Credential { Username = "admin", Password = "SecRet P@ssw0rd" };

		public string Username { get; set; }

		public string Password { get; set; }
	}
}
