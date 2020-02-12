using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWebFormsComponents.Enums
{
	public abstract class ValidationDataType
	{
		public static StringDataType String { get; } = new StringDataType();
		public static IntegerDataType Integer { get; } = new IntegerDataType();
		public static DoubleDataType Double { get; } = new DoubleDataType();
		public static DateDataType Date { get; } = new DateDataType();
		public static CurrencyDataType Currency { get; } = new CurrencyDataType();
	}

	public class StringDataType : ValidationDataType { }
	public class IntegerDataType : ValidationDataType { }
	public class DoubleDataType : ValidationDataType { }
	public class DateDataType : ValidationDataType { }
	public class CurrencyDataType : ValidationDataType { }
}
