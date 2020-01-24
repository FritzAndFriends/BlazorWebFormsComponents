using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWebFormsComponents.Enums
{
	public abstract class ValidationSummaryDisplayMode
	{

		public static ListDisplayMode List { get; } = new ListDisplayMode();
		public static BulletListDisplayMode BulletList { get; } = new BulletListDisplayMode();
		public static SingleParagraphDisplayMode SingleParagraph { get; } = new SingleParagraphDisplayMode();

	}

	public class ListDisplayMode : ValidationSummaryDisplayMode { }
	public class BulletListDisplayMode : ValidationSummaryDisplayMode { }
	public class SingleParagraphDisplayMode : ValidationSummaryDisplayMode { }
}
