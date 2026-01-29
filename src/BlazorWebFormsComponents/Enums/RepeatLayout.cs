namespace BlazorWebFormsComponents.Enums
{
	public abstract class RepeatLayout
	{
		public static TableRepeatLayout Table { get; } = new TableRepeatLayout();
		public static FlowRepeatLayout Flow { get; } = new FlowRepeatLayout();
		public static OrderedListRepeatLayout OrderedList { get; } = new OrderedListRepeatLayout();
		public static UnorderedListRepeatLayout UnorderedList { get; } = new UnorderedListRepeatLayout();
	}
	public class TableRepeatLayout : RepeatLayout { }
	public class FlowRepeatLayout : RepeatLayout { }
	public class OrderedListRepeatLayout : RepeatLayout { }
	public class UnorderedListRepeatLayout : RepeatLayout { }
}
