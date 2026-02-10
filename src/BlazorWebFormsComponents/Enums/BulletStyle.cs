namespace BlazorWebFormsComponents.Enums
{
	/// <summary>
	/// Specifies the bullet style of a BulletedList control.
	/// </summary>
	public enum BulletStyle
	{
		/// <summary>
		/// The bullet style is not set.
		/// </summary>
		NotSet = 0,

		/// <summary>
		/// The bullet is a filled circle.
		/// </summary>
		Disc = 1,

		/// <summary>
		/// The bullet is an empty circle.
		/// </summary>
		Circle = 2,

		/// <summary>
		/// The bullet is a filled square.
		/// </summary>
		Square = 3,

		/// <summary>
		/// The bullet is a number (1, 2, 3, ...).
		/// </summary>
		Numbered = 4,

		/// <summary>
		/// The bullet is a lowercase letter (a, b, c, ...).
		/// </summary>
		LowerAlpha = 5,

		/// <summary>
		/// The bullet is an uppercase letter (A, B, C, ...).
		/// </summary>
		UpperAlpha = 6,

		/// <summary>
		/// The bullet is a lowercase Roman numeral (i, ii, iii, ...).
		/// </summary>
		LowerRoman = 7,

		/// <summary>
		/// The bullet is an uppercase Roman numeral (I, II, III, ...).
		/// </summary>
		UpperRoman = 8,

		/// <summary>
		/// The bullet is a custom image specified by the BulletImageUrl property.
		/// </summary>
		CustomImage = 9
	}
}
