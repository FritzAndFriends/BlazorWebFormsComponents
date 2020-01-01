using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace BlazorWebFormsComponents
{
	public partial class DataList<ItemType> : BaseModelBindingComponent<ItemType>
	{

		[Parameter]
		public RenderFragment HeaderTemplate { get; set; }

		[Parameter]
		public RenderFragment<ItemType> ItemTemplate { get; set; }

		[Parameter]
		public RepeatLayout RepeatLayout { get; set; } = BlazorWebFormsComponents.RepeatLayout.Table;

	}

	public abstract class RepeatLayout {

		public static TableRepeatLayout Table => new TableRepeatLayout();
		public static FlowRepeatLayout Flow => new FlowRepeatLayout();

	}

	public class TableRepeatLayout : RepeatLayout { }
	public class FlowRepeatLayout : RepeatLayout { }

}
