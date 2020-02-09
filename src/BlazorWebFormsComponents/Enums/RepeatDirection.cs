namespace BlazorWebFormsComponents.Enums
{
  public abstract class RepeatDirection
	{
		public static HorizontalRepeatDirection Horizontal { get; } = new HorizontalRepeatDirection();
		public static VerticalRepeatDirection Vertical { get; } = new VerticalRepeatDirection();
	}
	public class HorizontalRepeatDirection : RepeatDirection { }
	public class VerticalRepeatDirection : RepeatDirection { }

}
