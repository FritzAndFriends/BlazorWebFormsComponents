using System;

namespace BlazorAjaxToolkitComponents.Enums;

/// <summary>
/// Specifies the types of characters to allow in a FilteredTextBoxExtender.
/// This is a flags enum — values can be combined.
/// </summary>
[Flags]
public enum FilterType
{
	Custom = 0,
	Numbers = 1,
	LowercaseLetters = 2,
	UppercaseLetters = 4
}
