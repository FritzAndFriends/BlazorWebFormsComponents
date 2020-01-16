using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWebFormsComponents.Enums
{
	public class ButtonType
	{

		public static ButtonButtonType Button => new ButtonButtonType();
		public static ImageButtonType Image => new ImageButtonType();
		public static LinkButtonType Link => new LinkButtonType();

	}

	public class ButtonButtonType : ButtonType { }
	public class ImageButtonType : ButtonType { }
	public class LinkButtonType : ButtonType { }
}
