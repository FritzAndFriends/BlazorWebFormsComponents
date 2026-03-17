using BlazorWebFormsComponents.DataBinding;
using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{

	public partial class ListView<ItemType> : DataBoundComponent<ItemType>
	{

		public ListView()
		{
		}

		#region Templates
		[Parameter] public RenderFragment EmptyDataTemplate { get; set; }
		[Parameter] public RenderFragment<ItemType> ItemTemplate { get; set; }
		[Parameter] public RenderFragment<ItemType> AlternatingItemTemplate { get; set; }
		[Parameter] public RenderFragment ItemSeparatorTemplate { get; set; }
		[Parameter] public RenderFragment GroupSeparatorTemplate { get; set; }
		[Parameter] public RenderFragment<RenderFragment> GroupTemplate { get; set; }
		[Parameter] public RenderFragment ItemPlaceHolder { get; set; }

		/// <summary>
		/// Template rendered for the item currently in edit mode.
		/// </summary>
		[Parameter] public RenderFragment<ItemType> EditItemTemplate { get; set; }

		/// <summary>
		/// Template rendered for inserting a new item.
		/// </summary>
		[Parameter] public RenderFragment<ItemType> InsertItemTemplate { get; set; }

		/// <summary>
		/// Template rendered when the data source contains no items.
		/// </summary>
		[Parameter] public RenderFragment EmptyItemTemplate { get; set; }

		/// <summary>
		/// The layout of the ListView, a set of HTML to contain the repeated elements of the ItemTemplate and AlternativeItemTemplate 
		/// </summary>
		[Parameter]
		public RenderFragment<RenderFragment> LayoutTemplate { get; set; }

		/// <summary>
		/// Occurs after the layout template has been created.
		/// </summary>
		[Parameter]
		public EventCallback<EventArgs> OnLayoutCreated { get; set; }

		#endregion

		#region Properties

		[Parameter] public int GroupItemCount { get; set; } = 0;

		/// <summary>
		/// Gets or sets the position of the insert item template. Controls whether the insert row appears at the top, bottom, or not at all.
		/// </summary>
		[Parameter]
		public EnumParameter<InsertItemPosition> InsertItemPosition { get; set; }

		/// <summary>
		/// Gets or sets the index of the item currently in edit mode. -1 means no item is being edited.
		/// </summary>
		[Parameter] public int EditIndex { get; set; } = -1;

		[Parameter]
		public int SelectedIndex { get; set; }

		/// <summary>
		/// Gets or sets the current sort expression.
		/// </summary>
		[Parameter]
		public string SortExpression { get; set; }

		/// <summary>
		/// Gets or sets the current sort direction.
		/// </summary>
		[Parameter]
		public EnumParameter<SortDirection> SortDirection { get; set; } = Enums.SortDirection.Ascending;

		/// <summary>
		/// Gets or sets the index of the first record to display on a page.
		/// </summary>
		[Parameter]
		public int StartRowIndex { get; set; }

		/// <summary>
		/// Gets or sets the maximum number of items to display on a single page.
		/// </summary>
		[Parameter]
		public int MaximumRows { get; set; } = int.MaxValue;

		/// <summary>
		/// Style is not applied by this control
		/// </summary>
		[Parameter, Obsolete("Style is not applied by this control")]
		public new string Style { get; set; }

		#endregion

		[Parameter] public RenderFragment ChildContent { get; set; }

		#region Events

		[Parameter]
		public EventCallback<ListViewItemEventArgs> OnItemDataBound { get; set; }

		/// <summary>
		/// Occurs when a command button within the ListView is clicked.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewCommandEventArgs> ItemCommand { get; set; }

		/// <summary>
		/// Web Forms migration alias for ItemCommand.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewCommandEventArgs> OnItemCommand { get; set; }

		/// <summary>
		/// Occurs when an Edit command is requested, before the item enters edit mode.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewEditEventArgs> ItemEditing { get; set; }

		/// <summary>
		/// Web Forms migration alias for ItemEditing.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewEditEventArgs> OnItemEditing { get; set; }

		/// <summary>
		/// Occurs when a Cancel command is requested.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewCancelEventArgs> ItemCanceling { get; set; }

		/// <summary>
		/// Web Forms migration alias for ItemCanceling.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewCancelEventArgs> OnItemCanceling { get; set; }

		/// <summary>
		/// Occurs when a Delete command is requested, before the item is deleted.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewDeleteEventArgs> ItemDeleting { get; set; }

		/// <summary>
		/// Web Forms migration alias for ItemDeleting.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewDeleteEventArgs> OnItemDeleting { get; set; }

		/// <summary>
		/// Occurs after the delete operation completes.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewDeletedEventArgs> ItemDeleted { get; set; }

		/// <summary>
		/// Web Forms migration alias for ItemDeleted.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewDeletedEventArgs> OnItemDeleted { get; set; }

		/// <summary>
		/// Occurs when an Insert command is requested, before the insert operation.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewInsertEventArgs> ItemInserting { get; set; }

		/// <summary>
		/// Web Forms migration alias for ItemInserting.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewInsertEventArgs> OnItemInserting { get; set; }

		/// <summary>
		/// Occurs after the insert operation completes.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewInsertedEventArgs> ItemInserted { get; set; }

		/// <summary>
		/// Web Forms migration alias for ItemInserted.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewInsertedEventArgs> OnItemInserted { get; set; }

		/// <summary>
		/// Occurs when an Update command is requested, before the update operation.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewUpdateEventArgs> ItemUpdating { get; set; }

		/// <summary>
		/// Web Forms migration alias for ItemUpdating.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewUpdateEventArgs> OnItemUpdating { get; set; }

		/// <summary>
		/// Occurs after the update operation completes.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewUpdatedEventArgs> ItemUpdated { get; set; }

		/// <summary>
		/// Web Forms migration alias for ItemUpdated.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewUpdatedEventArgs> OnItemUpdated { get; set; }

		/// <summary>
		/// Occurs when an item is created in the ListView control, before data binding.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewItemEventArgs> ItemCreated { get; set; }

		/// <summary>
		/// Web Forms migration alias for ItemCreated.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewItemEventArgs> OnItemCreated { get; set; }

		/// <summary>
		/// Occurs before a sort operation. Can be cancelled.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewSortEventArgs> Sorting { get; set; }

		/// <summary>
		/// Web Forms migration alias for Sorting.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewSortEventArgs> OnSorting { get; set; }

		/// <summary>
		/// Occurs after a sort operation completes.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewSortEventArgs> Sorted { get; set; }

		/// <summary>
		/// Web Forms migration alias for Sorted.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewSortEventArgs> OnSorted { get; set; }

		/// <summary>
		/// Occurs when the page properties are changing.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewPagePropertiesChangingEventArgs> PagePropertiesChanging { get; set; }

		/// <summary>
		/// Web Forms migration alias for PagePropertiesChanging.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewPagePropertiesChangingEventArgs> OnPagePropertiesChanging { get; set; }

		/// <summary>
		/// Occurs after the page properties have changed.
		/// </summary>
		[Parameter]
		public EventCallback<EventArgs> PagePropertiesChanged { get; set; }

		/// <summary>
		/// Web Forms migration alias for PagePropertiesChanged.
		/// </summary>
		[Parameter]
		public EventCallback<EventArgs> OnPagePropertiesChanged { get; set; }

		/// <summary>
		/// Occurs before the selected index changes. Can be cancelled.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewSelectEventArgs> SelectedIndexChanging { get; set; }

		/// <summary>
		/// Web Forms migration alias for SelectedIndexChanging.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewSelectEventArgs> OnSelectedIndexChanging { get; set; }

		/// <summary>
		/// Occurs after the selected index has changed.
		/// </summary>
		[Parameter]
		public EventCallback<EventArgs> SelectedIndexChanged { get; set; }

		/// <summary>
		/// Web Forms migration alias for SelectedIndexChanged.
		/// </summary>
		[Parameter]
		public EventCallback<EventArgs> OnSelectedIndexChanged { get; set; }

		#endregion

		[CascadingParameter(Name = "Host")] public BaseWebFormsComponent HostComponent { get; set; }

		protected override void OnInitialized()
		{
			HostComponent = this;
			base.OnInitialized();
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			await base.OnAfterRenderAsync(firstRender);
		}

		protected virtual void ItemDataBound(ListViewItemEventArgs e)
		{
			OnItemDataBound.InvokeAsync(e);
		}

		/// <summary>
		/// Raises the ItemCreated event for the specified item.
		/// In Web Forms, this fires for each item before data binding.
		/// </summary>
		internal void RaiseItemCreated(ListViewItem item)
		{
			if (ItemCreated.HasDelegate || OnItemCreated.HasDelegate)
			{
				var handler = ItemCreated.HasDelegate ? ItemCreated : OnItemCreated;
				_ = handler.InvokeAsync(new ListViewItemEventArgs(item) { Sender = this });
			}
		}

		/// <summary>
		/// Invokes the LayoutCreated event. Called after the layout template has been rendered.
		/// </summary>
		internal void RaiseLayoutCreated()
		{
			if (OnLayoutCreated.HasDelegate)
			{
				_ = OnLayoutCreated.InvokeAsync(EventArgs.Empty);
			}
		}

		#region Command Handling

		/// <summary>
		/// Routes a command to the appropriate event handler based on command name.
		/// In Web Forms, ItemCommand fires first for every command, then the specific handler runs.
		/// </summary>
		public async Task HandleCommand(string commandName, object commandArgument, int itemIndex)
		{
			// Web Forms always fires ItemCommand first, regardless of command type
			var cmdArgs = new ListViewCommandEventArgs(commandName, commandArgument);
			var itemCommandHandler = ItemCommand.HasDelegate ? ItemCommand : OnItemCommand;
			await itemCommandHandler.InvokeAsync(cmdArgs);

			switch (commandName.ToLowerInvariant())
			{
				case "edit":
					await HandleEditCommand(itemIndex);
					break;
				case "cancel":
					await HandleCancelCommand(itemIndex);
					break;
				case "delete":
					await HandleDeleteCommand(itemIndex);
					break;
				case "insert":
					await HandleInsertCommand();
					break;
				case "update":
					await HandleUpdateCommand(itemIndex);
					break;
				case "sort":
					await HandleSortCommand(commandArgument as string ?? string.Empty);
					break;
				case "select":
					await HandleSelectCommand(itemIndex);
					break;
			}

			StateHasChanged();
		}

		private async Task HandleEditCommand(int itemIndex)
		{
			var args = new ListViewEditEventArgs(itemIndex);
			var itemEditingHandler = ItemEditing.HasDelegate ? ItemEditing : OnItemEditing;
			await itemEditingHandler.InvokeAsync(args);
			if (args.Cancel) return;
			EditIndex = args.NewEditIndex;
		}

		private async Task HandleCancelCommand(int itemIndex)
		{
			var cancelMode = InsertItemPosition.Value != Enums.InsertItemPosition.None && EditIndex < 0
				? ListViewCancelMode.CancelingInsert
				: ListViewCancelMode.CancelingEdit;

			var args = new ListViewCancelEventArgs(itemIndex, cancelMode);
			var itemCancelingHandler = ItemCanceling.HasDelegate ? ItemCanceling : OnItemCanceling;
			await itemCancelingHandler.InvokeAsync(args);
			if (args.Cancel) return;

			if (cancelMode == ListViewCancelMode.CancelingEdit)
			{
				EditIndex = -1;
			}
		}

		private async Task HandleDeleteCommand(int itemIndex)
		{
			var deletingArgs = new ListViewDeleteEventArgs(itemIndex);
			var itemDeletingHandler = ItemDeleting.HasDelegate ? ItemDeleting : OnItemDeleting;
			await itemDeletingHandler.InvokeAsync(deletingArgs);
			if (deletingArgs.Cancel) return;

			Exception caughtException = null;
			try
			{
				// The actual deletion is expected to be handled by the consumer in ItemDeleting
			}
			catch (Exception ex)
			{
				caughtException = ex;
			}

			var deletedArgs = new ListViewDeletedEventArgs(1, caughtException);
			var itemDeletedHandler = ItemDeleted.HasDelegate ? ItemDeleted : OnItemDeleted;
			await itemDeletedHandler.InvokeAsync(deletedArgs);
		}

		private async Task HandleInsertCommand()
		{
			var insertingArgs = new ListViewInsertEventArgs();
			var itemInsertingHandler = ItemInserting.HasDelegate ? ItemInserting : OnItemInserting;
			await itemInsertingHandler.InvokeAsync(insertingArgs);
			if (insertingArgs.Cancel) return;

			Exception caughtException = null;
			try
			{
				// The actual insertion is expected to be handled by the consumer in ItemInserting
			}
			catch (Exception ex)
			{
				caughtException = ex;
			}

			var insertedArgs = new ListViewInsertedEventArgs(1, caughtException);
			var itemInsertedHandler = ItemInserted.HasDelegate ? ItemInserted : OnItemInserted;
			await itemInsertedHandler.InvokeAsync(insertedArgs);
		}

		private async Task HandleUpdateCommand(int itemIndex)
		{
			var updatingArgs = new ListViewUpdateEventArgs(itemIndex);
			var itemUpdatingHandler = ItemUpdating.HasDelegate ? ItemUpdating : OnItemUpdating;
			await itemUpdatingHandler.InvokeAsync(updatingArgs);
			if (updatingArgs.Cancel) return;

			Exception caughtException = null;
			try
			{
				// The actual update is expected to be handled by the consumer in ItemUpdating
			}
			catch (Exception ex)
			{
				caughtException = ex;
			}

			var updatedArgs = new ListViewUpdatedEventArgs(1, caughtException);
			var itemUpdatedHandler = ItemUpdated.HasDelegate ? ItemUpdated : OnItemUpdated;
			await itemUpdatedHandler.InvokeAsync(updatedArgs);

			if (!updatedArgs.KeepInEditMode)
			{
				EditIndex = -1;
			}
		}

		private async Task HandleSortCommand(string sortExpression)
		{
			var newDirection = (sortExpression == SortExpression && SortDirection.Value == Enums.SortDirection.Ascending)
				? Enums.SortDirection.Descending
				: Enums.SortDirection.Ascending;

			var args = new ListViewSortEventArgs(sortExpression, newDirection);
			var sortingHandler = Sorting.HasDelegate ? Sorting : OnSorting;
			await sortingHandler.InvokeAsync(args);
			if (args.Cancel) return;

			SortExpression = args.SortExpression;
			SortDirection = args.SortDirection;
			var sortedHandler = Sorted.HasDelegate ? Sorted : OnSorted;
			await sortedHandler.InvokeAsync(args);
		}

		private async Task HandleSelectCommand(int itemIndex)
		{
			var args = new ListViewSelectEventArgs(itemIndex);
			var selectedIndexChangingHandler = SelectedIndexChanging.HasDelegate ? SelectedIndexChanging : OnSelectedIndexChanging;
			await selectedIndexChangingHandler.InvokeAsync(args);
			if (args.Cancel) return;

			SelectedIndex = args.NewSelectedIndex;
			var selectedIndexChangedHandler = SelectedIndexChanged.HasDelegate ? SelectedIndexChanged : OnSelectedIndexChanged;
			await selectedIndexChangedHandler.InvokeAsync(EventArgs.Empty);
		}

		#endregion

		/// <summary>
		/// Sets the page properties and fires the PagePropertiesChanging/Changed events.
		/// </summary>
		public async Task SetPageProperties(int startRowIndex, int maximumRows)
		{
			var args = new ListViewPagePropertiesChangingEventArgs(startRowIndex, maximumRows);
			var pagePropertiesChangingHandler = PagePropertiesChanging.HasDelegate ? PagePropertiesChanging : OnPagePropertiesChanging;
			await pagePropertiesChangingHandler.InvokeAsync(args);

			StartRowIndex = args.StartRowIndex;
			MaximumRows = args.MaximumRows;

			var pagePropertiesChangedHandler = PagePropertiesChanged.HasDelegate ? PagePropertiesChanged : OnPagePropertiesChanged;
			await pagePropertiesChangedHandler.InvokeAsync(EventArgs.Empty);
			StateHasChanged();
		}

		/// <summary>
		/// Gets the appropriate template for the item at the specified index.
		/// Returns EditItemTemplate when the item is at EditIndex, otherwise the standard item template.
		/// </summary>
		internal RenderFragment<ItemType> GetItemTemplate(int itemIndex, bool isEven)
		{
			if (EditIndex >= 0 && itemIndex == EditIndex && EditItemTemplate != null)
			{
				return EditItemTemplate;
			}
			return isEven ? ItemTemplate : AlternatingItemTemplate ?? ItemTemplate;
		}
	}
}
