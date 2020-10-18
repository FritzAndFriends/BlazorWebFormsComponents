namespace BlazorWebFormsComponents
{
	public sealed class FontInfo
	{
		public bool Bold { get; set; }

		public bool Italic { get; set; }

		public string Name { get; set; }

		public string Names { get; set; }

		public bool Overline { get; set; }

		public FontUnit Size { get; set; } = FontUnit.Empty;

		public bool Strikeout { get; set; }

		public bool Underline { get; set; }
	}
}
