using System;

namespace BlazorAjaxToolkitComponents.Enums;

/// <summary>
/// Specifies which corners of a box should be rounded.
/// This is a flags enum — values can be combined.
/// </summary>
[Flags]
public enum BoxCorners
{
	/// <summary>
	/// No corners are rounded.
	/// </summary>
	None = 0,

	/// <summary>
	/// Top-left corner is rounded.
	/// </summary>
	TopLeft = 1,

	/// <summary>
	/// Top-right corner is rounded.
	/// </summary>
	TopRight = 2,

	/// <summary>
	/// Bottom-left corner is rounded.
	/// </summary>
	BottomLeft = 4,

	/// <summary>
	/// Bottom-right corner is rounded.
	/// </summary>
	BottomRight = 8,

	/// <summary>
	/// Both top corners are rounded.
	/// </summary>
	Top = TopLeft | TopRight,

	/// <summary>
	/// Both bottom corners are rounded.
	/// </summary>
	Bottom = BottomLeft | BottomRight,

	/// <summary>
	/// Both left corners are rounded.
	/// </summary>
	Left = TopLeft | BottomLeft,

	/// <summary>
	/// Both right corners are rounded.
	/// </summary>
	Right = TopRight | BottomRight,

	/// <summary>
	/// All four corners are rounded.
	/// </summary>
	All = TopLeft | TopRight | BottomLeft | BottomRight
}
