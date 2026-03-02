namespace BlazorWebFormsComponents
{
	public sealed class FontInfo
	{
		private string _name;
		private string _names;

		public bool Bold { get; set; }

		public bool Italic { get; set; }

		/// <summary>
		/// Auto-syncs with <see cref="Names"/>: setting Name also sets Names,
		/// matching ASP.NET Web Forms FontInfo behavior.
		/// </summary>
		public string Name
		{
			get => _name;
			set
			{
				_name = value;
				if (!string.IsNullOrEmpty(value))
					_names = value;
				else
					_names = null;
			}
		}

		/// <summary>
		/// Comma-separated font names. Auto-syncs with <see cref="Name"/>:
		/// setting Names also sets Name to the first entry.
		/// </summary>
		public string Names
		{
			get => _names;
			set
			{
				_names = value;
				if (!string.IsNullOrEmpty(value))
				{
					var idx = value.IndexOf(',');
					_name = idx >= 0 ? value.Substring(0, idx).Trim() : value.Trim();
				}
				else
				{
					_name = null;
				}
			}
		}

		public bool Overline { get; set; }

		public FontUnit Size { get; set; } = FontUnit.Empty;

		public bool Strikeout { get; set; }

		public bool Underline { get; set; }
	}
}
