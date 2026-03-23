using System;

namespace BlazorWebFormsComponents.Enums;

/// <summary>
/// Specifies anti-aliasing flags. Matches the ASP.NET Web Forms AntiAliasingStyles enum.
/// </summary>
[Flags]
public enum AntiAliasingStyles
{
	None = 0,
	Graphics = 1,
	Text = 2,
	All = Graphics | Text
}
