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
		public string RepeatLayout { get; set; } = BlazorWebFormsComponents.RepeatLayout.Table;

	}

	public sealed class RepeatLayout {
		
		private RepeatLayout() { }

		private static readonly RepeatLayout[] _Instances = new RepeatLayout[] {
			new RepeatLayout { Markup = nameof(Table) },
			new RepeatLayout { Markup = nameof(Flow) }
		};

		internal string Markup { get; private set; }

		public static RepeatLayout Table
		{
			get
			{
				return _Instances[0];
			}
		}

		public static RepeatLayout Flow
		{
			get
			{
				return _Instances[1];
			}
		}

		public static implicit operator RepeatLayout(string theString) {

			switch (theString.ToLowerInvariant()) {
				case "table":
					return Table;
				case "flow":
					return Flow;
			}

			return null;

		}

		public static implicit operator String(RepeatLayout layout) {
			return layout.Markup;
		}

	}

}
