namespace BlazorWebFormsComponents.Enums
{
  public abstract class DataListEnum
	{
		public static HorizontalDataList Horizontal { get; } = new HorizontalDataList();
		public static VerticalDataList Vertical { get; } = new VerticalDataList();
		public static NoneDataList None { get; } = new NoneDataList();
		public static BothDataList Both { get; } = new BothDataList();
	}
	public class HorizontalDataList : DataListEnum { }
	public class VerticalDataList : DataListEnum { }
	public class NoneDataList : DataListEnum { }
	public class BothDataList : DataListEnum { }
}
