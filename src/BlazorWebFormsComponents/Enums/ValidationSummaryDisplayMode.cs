using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWebFormsComponents.Enums
{
	public abstract class ValidationSummaryDisplayMode
	{

		public static ListDisplayMode List => new ListDisplayMode();
		public static BulletListDisplayMode BulletList => new BulletListDisplayMode();
		public static SingleParagraphDisplayMode SingleParagraph => new SingleParagraphDisplayMode();

	}

	public class ListDisplayMode : ValidationSummaryDisplayMode { }
	public class BulletListDisplayMode : ValidationSummaryDisplayMode { }
	public class SingleParagraphDisplayMode : ValidationSummaryDisplayMode { }
}
