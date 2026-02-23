using BlazorWebFormsComponents.DataBinding;
using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{

	public partial class FormView<ItemType> : DataBoundComponent<ItemType> where ItemType : class, new()
	{

		private static readonly Dictionary<string, FormViewMode> CommandNameModeLookup = new Dictionary<string, FormViewMode> {
			{ "delete", FormViewMode.ReadOnly },
			{ "edit", FormViewMode.Edit },
			{ "insert", FormViewMode.ReadOnly },
			{ "new", FormViewMode.Insert },
			{ "update", FormViewMode.ReadOnly }
		};

		public FormView()
		{

			// Listen for bubbled events
			BubbledEvent += FormView_BubbledEvent;

		}

		[Parameter]
		public RenderFragment<ItemType> EditItemTemplate { get; set; }

		[Parameter]
		public RenderFragment<ItemType> InsertItemTemplate { get; set; }

		[Parameter]
		public RenderFragment<ItemType> ItemTemplate { get; set; }

		/// <summary>
		/// Gets or sets the text displayed in the header row.
		/// </summary>
		[Parameter]
		public string HeaderText { get; set; }

		/// <summary>
		/// Gets or sets the custom header template. Takes precedence over HeaderText.
		/// </summary>
		[Parameter]
		public RenderFragment HeaderTemplate { get; set; }

		/// <summary>
		/// Gets or sets the text displayed in the footer row.
		/// </summary>
		[Parameter]
		public string FooterText { get; set; }

		/// <summary>
		/// Gets or sets the custom footer template. Takes precedence over FooterText.
		/// </summary>
		[Parameter]
		public RenderFragment FooterTemplate { get; set; }

		/// <summary>
		/// Gets or sets the text shown when the DataSource is empty or null.
		/// </summary>
		[Parameter]
		public string EmptyDataText { get; set; }

		/// <summary>
		/// Gets or sets the custom template for the empty state. Takes precedence over EmptyDataText.
		/// </summary>
		[Parameter]
		public RenderFragment EmptyDataTemplate { get; set; }

		public ItemType CurrentItem { get; set; }

		public FormViewMode CurrentMode { get; private set; }

		[Parameter]
		public FormViewMode DefaultMode { get; set; } = FormViewMode.ReadOnly;

		private int _Position = 1;
		protected int Position
		{
			get { return _Position; }
			set
			{
				_Position = value;
				CurrentItem = Items.Skip(value - 1).FirstOrDefault();
			}
		}

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{

			if (firstRender)
			{
				if ((CurrentItem is null) && Items != null && Items.Any()) Position = 1;
			}

			await base.OnAfterRenderAsync(firstRender);

		}

		#region FormView Events

		protected override Task OnInitializedAsync()
		{

			CurrentMode = DefaultMode;

			return base.OnInitializedAsync();
		}

		[Parameter]
		public EventCallback<FormViewModeEventArgs> ModeChanging { get; set; }

		private void FormView_BubbledEvent(object sender, System.EventArgs e)
		{

			if (e is CommandEventArgs args) HandleCommandArgs(args);

		}

		private void HandleCommandArgs(CommandEventArgs args)
		{

			switch (args.CommandName.ToLowerInvariant())
			{
				case "cancel":
					ModeChanging.InvokeAsync(new FormViewModeEventArgs() { NewMode = DefaultMode, Sender = this }).GetAwaiter().GetResult();
					CurrentMode = DefaultMode;
					break;
				case "edit":
					ModeChanging.InvokeAsync(new FormViewModeEventArgs() { NewMode = FormViewMode.Edit, Sender = this }).GetAwaiter().GetResult();
					CurrentMode = FormViewMode.Edit;
					break;
				case "delete":
					Exception caughtException = null;
					try {
						OnItemDeleting.InvokeAsync(new FormViewDeleteEventArgs(Position) { Sender = this }).GetAwaiter().GetResult();
					} catch (Exception ex) {
						caughtException = ex;
					}
					// do we do the deletion?
					OnItemDeleted.InvokeAsync(new FormViewDeletedEventArgs(Position, caughtException) { Sender = this }).GetAwaiter().GetResult();
					CurrentMode = DefaultMode;
					Position = (Position == 0) ? 0 : Position - 1;
					break;
				case "insert":
					OnItemInserting.InvokeAsync(new FormViewInsertEventArgs("insert") { Sender = this }).GetAwaiter().GetResult();
					ModeChanging.InvokeAsync(new FormViewModeEventArgs() { NewMode = FormViewMode.Insert, Sender = this }).GetAwaiter().GetResult();
					OnItemInserted.InvokeAsync(new FormViewInsertEventArgs("insert") { Sender = this }).GetAwaiter().GetResult();
					CurrentMode = FormViewMode.Insert;
					break;
				case "update":
					OnItemUpdating.InvokeAsync(new FormViewUpdateEventArgs("update") { Sender = this }).GetAwaiter().GetResult();
					ModeChanging.InvokeAsync(new FormViewModeEventArgs() { NewMode = DefaultMode, Sender = this }).GetAwaiter().GetResult();
					OnItemUpdated.InvokeAsync(new FormViewUpdatedEventArgs(0, null) { Sender = this }).GetAwaiter().GetResult();
					CurrentMode = DefaultMode;
					break;

			}

			StateHasChanged();

		}

		#endregion

		#region Custom Events

		[Parameter]
		public EventCallback<FormViewDeleteEventArgs> OnItemDeleting { get; set; }

		[Parameter]
		public EventCallback<FormViewDeletedEventArgs> OnItemDeleted { get; set; }

		[Parameter]
		public EventCallback<FormViewInsertEventArgs> OnItemInserting { get; set; }

		[Parameter]
		public EventCallback<FormViewInsertEventArgs> OnItemInserted { get; set; }

		[Parameter]
		public EventCallback<FormViewUpdateEventArgs> OnItemUpdating { get; set; }

		[Parameter]
		public EventCallback<FormViewUpdatedEventArgs> OnItemUpdated { get; set; }

		#endregion

	}

}
