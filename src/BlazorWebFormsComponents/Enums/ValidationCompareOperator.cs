using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWebFormsComponents.Enums
{
	public abstract class ValidationCompareOperator
	{
		public static EqualCompareOperator Equal { get; } = new EqualCompareOperator();
		public static NotEqualCompareOperator NotEqual { get; } = new NotEqualCompareOperator();
		public static GreaterThanCompareOperator GreaterThan { get; } = new GreaterThanCompareOperator();
		public static GreaterThanEqualCompareOperator GreaterThanEqual { get; } = new GreaterThanEqualCompareOperator();
		public static LessThanCompareOperator LessThan { get; } = new LessThanCompareOperator();
		public static LessThanEqualCompareOperator LessThanEqual { get; } = new LessThanEqualCompareOperator();
		public static DataTypeCheckCompareOperator DataTypeCheck { get; } = new DataTypeCheckCompareOperator();
	}

	public class EqualCompareOperator : ValidationCompareOperator { }
	public class NotEqualCompareOperator : ValidationCompareOperator { }
	public class GreaterThanCompareOperator : ValidationCompareOperator { }
	public class GreaterThanEqualCompareOperator : ValidationCompareOperator { }
	public class LessThanCompareOperator : ValidationCompareOperator { }
	public class LessThanEqualCompareOperator : ValidationCompareOperator { }
	public class DataTypeCheckCompareOperator : ValidationCompareOperator { }
}
