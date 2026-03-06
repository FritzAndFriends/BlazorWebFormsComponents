using BlazorWebFormsComponents.DataBinding;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class Repeater<ItemType> : DataBoundComponent<ItemType>
	{

		public Repeater()
		{
			BubbledEvent += Repeater_BubbledEvent;
		}

		private void Repeater_BubbledEvent(object sender, System.EventArgs e)
		{
			if (e is CommandEventArgs cmd)
			{
				var args = new RepeaterCommandEventArgs(cmd.CommandName, cmd.CommandArgument, null) { Sender = cmd.Sender ?? this };
				var handler = OnItemCommand.HasDelegate ? OnItemCommand : ItemCommand;
				if (handler.HasDelegate) { _ = handler.InvokeAsync(args); }
			}
		}

		protected override void OnParametersSet()
		{
			base.OnParametersSet();
			FireItemEvents();
		}

		private void FireItemEvents()
		{
			if (Items == null) return;

			var createdHandler = OnItemCreated.HasDelegate ? OnItemCreated : ItemCreated;
			var boundHandler = OnItemDataBound.HasDelegate ? OnItemDataBound : ItemDataBound;

			foreach (var item in Items)
			{
				if (createdHandler.HasDelegate)
				{
					_ = createdHandler.InvokeAsync(new RepeaterItemEventArgs(item) { Sender = this });
				}

				if (boundHandler.HasDelegate)
				{
					_ = boundHandler.InvokeAsync(new RepeaterItemEventArgs(item) { Sender = this });
				}
			}
		}

		[Parameter]
		public RenderFragment<ItemType> ItemTemplate { get; set; }

		[Parameter]
		public RenderFragment<ItemType> AlternatingItemTemplate { get; set; }

		[Parameter]
		public RenderFragment HeaderTemplate { get; set; }

		[Parameter]
		public RenderFragment FooterTemplate { get; set; }

		[Parameter]
		public RenderFragment SeparatorTemplate { get; set; }

		#region Events

		/// <summary>
		/// Occurs when a command button within the Repeater is clicked.
		/// </summary>
		[Parameter]
		public EventCallback<RepeaterCommandEventArgs> ItemCommand { get; set; }

		/// <summary>
		/// Web Forms migration alias for ItemCommand.
		/// </summary>
		[Parameter]
		public EventCallback<RepeaterCommandEventArgs> OnItemCommand { get; set; }

		/// <summary>
		/// Occurs when an item is created in the Repeater control.
		/// </summary>
		[Parameter]
		public EventCallback<RepeaterItemEventArgs> ItemCreated { get; set; }

		/// <summary>
		/// Web Forms migration alias for ItemCreated.
		/// </summary>
		[Parameter]
		public EventCallback<RepeaterItemEventArgs> OnItemCreated { get; set; }

		/// <summary>
		/// Occurs when an item is data-bound in the Repeater control.
		/// </summary>
		[Parameter]
		public EventCallback<RepeaterItemEventArgs> ItemDataBound { get; set; }

		/// <summary>
		/// Web Forms migration alias for ItemDataBound.
		/// </summary>
		[Parameter]
		public EventCallback<RepeaterItemEventArgs> OnItemDataBound { get; set; }

		#endregion

	}
}
