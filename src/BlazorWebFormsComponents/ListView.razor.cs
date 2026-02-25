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
		/// Triggers when the layout template is completed construction 
		/// </summary>
		[Parameter]
		public EventHandler OnLayoutCreated { get; set; }

		#endregion

		#region Properties

		[Parameter] public int GroupItemCount { get; set; } = 0;

		/// <summary>
		/// Gets or sets the position of the insert item template. Controls whether the insert row appears at the top, bottom, or not at all.
		/// </summary>
		[Parameter]
		public InsertItemPosition InsertItemPosition { get; set; }

		/// <summary>
		/// Gets or sets the index of the item currently in edit mode. -1 means no item is being edited.
		/// </summary>
		[Parameter] public int EditIndex { get; set; } = -1;

		[Parameter]
		public int SelectedIndex { get; set; }

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
		/// Occurs when an Edit command is requested, before the item enters edit mode.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewEditEventArgs> ItemEditing { get; set; }

		/// <summary>
		/// Occurs when a Cancel command is requested.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewCancelEventArgs> ItemCanceling { get; set; }

		/// <summary>
		/// Occurs when a Delete command is requested, before the item is deleted.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewDeleteEventArgs> ItemDeleting { get; set; }

		/// <summary>
		/// Occurs after the delete operation completes.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewDeletedEventArgs> ItemDeleted { get; set; }

		/// <summary>
		/// Occurs when an Insert command is requested, before the insert operation.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewInsertEventArgs> ItemInserting { get; set; }

		/// <summary>
		/// Occurs after the insert operation completes.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewInsertedEventArgs> ItemInserted { get; set; }

		/// <summary>
		/// Occurs when an Update command is requested, before the update operation.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewUpdateEventArgs> ItemUpdating { get; set; }

		/// <summary>
		/// Occurs after the update operation completes.
		/// </summary>
		[Parameter]
		public EventCallback<ListViewUpdatedEventArgs> ItemUpdated { get; set; }

		/// <summary>
		/// Occurs when an item template is instantiated.
		/// </summary>
		[Parameter]
		public EventCallback ItemCreated { get; set; }

		#endregion

		[CascadingParameter(Name = "Host")] public BaseWebFormsComponent HostComponent { get; set; }

		protected override void OnInitialized()
		{
			HostComponent = this;
			base.OnInitialized();
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{
			if (firstRender && ItemCreated.HasDelegate)
			{
				await ItemCreated.InvokeAsync();
			}
			await base.OnAfterRenderAsync(firstRender);
		}

		protected virtual void ItemDataBound(ListViewItemEventArgs e)
		{
			OnItemDataBound.InvokeAsync(e);
		}

		#region Command Handling

		/// <summary>
		/// Routes a command to the appropriate event handler based on command name.
		/// </summary>
		public async Task HandleCommand(string commandName, object commandArgument, int itemIndex)
		{
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
				default:
					var cmdArgs = new ListViewCommandEventArgs(commandName, commandArgument);
					await ItemCommand.InvokeAsync(cmdArgs);
					break;
			}

			StateHasChanged();
		}

		private async Task HandleEditCommand(int itemIndex)
		{
			var args = new ListViewEditEventArgs(itemIndex);
			await ItemEditing.InvokeAsync(args);
			if (args.Cancel) return;
			EditIndex = args.NewEditIndex;
		}

		private async Task HandleCancelCommand(int itemIndex)
		{
			var cancelMode = InsertItemPosition != InsertItemPosition.None && EditIndex < 0
				? ListViewCancelMode.CancelingInsert
				: ListViewCancelMode.CancelingEdit;

			var args = new ListViewCancelEventArgs(itemIndex, cancelMode);
			await ItemCanceling.InvokeAsync(args);
			if (args.Cancel) return;

			if (cancelMode == ListViewCancelMode.CancelingEdit)
			{
				EditIndex = -1;
			}
		}

		private async Task HandleDeleteCommand(int itemIndex)
		{
			var deletingArgs = new ListViewDeleteEventArgs(itemIndex);
			await ItemDeleting.InvokeAsync(deletingArgs);
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
			await ItemDeleted.InvokeAsync(deletedArgs);
		}

		private async Task HandleInsertCommand()
		{
			var insertingArgs = new ListViewInsertEventArgs();
			await ItemInserting.InvokeAsync(insertingArgs);
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
			await ItemInserted.InvokeAsync(insertedArgs);
		}

		private async Task HandleUpdateCommand(int itemIndex)
		{
			var updatingArgs = new ListViewUpdateEventArgs(itemIndex);
			await ItemUpdating.InvokeAsync(updatingArgs);
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
			await ItemUpdated.InvokeAsync(updatedArgs);

			if (!updatedArgs.KeepInEditMode)
			{
				EditIndex = -1;
			}
		}

		#endregion

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
