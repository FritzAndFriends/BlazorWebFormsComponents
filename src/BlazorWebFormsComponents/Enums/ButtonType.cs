using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWebFormsComponents.Enums
{
	public abstract class ButtonType
	{

		public static ButtonButtonType Button { get; } = new ButtonButtonType();
		public static ImageButtonType Image { get; } = new ImageButtonType();
		public static LinkButtonType Link { get; } = new LinkButtonType();

	}

	public class ButtonButtonType : ButtonType { }
	public class ImageButtonType : ButtonType { }
	public class LinkButtonType : ButtonType { }
}
