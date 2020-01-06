using System;
using System.Collections.Generic;
using System.Text;

namespace BlazorWebFormsComponents.Enums
{
	public abstract class RepeatLayout
	{

		public static TableRepeatLayout Table => new TableRepeatLayout();
		public static FlowRepeatLayout Flow => new FlowRepeatLayout();

	}

	public class TableRepeatLayout : RepeatLayout { }
	public class FlowRepeatLayout : RepeatLayout { }

}
