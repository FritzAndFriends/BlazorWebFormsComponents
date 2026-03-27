namespace BlazorWebFormsComponents.Identity;

/// <summary>
/// Compatibility shim for <c>Microsoft.AspNet.Identity.Owin.SignInStatus</c>.
/// Represents the result of a sign-in attempt.
/// </summary>
public enum SignInStatus
{
    Success,
    LockedOut,
    RequiresVerification,
    Failure
}
