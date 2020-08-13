using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{

	public partial class FormView<ItemType> : BaseModelBindingComponent<ItemType> where ItemType : class, new()
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
		public RenderFragment<dynamic> EditItemTemplate { get; set; }

		[Parameter]
		public RenderFragment<dynamic> ItemTemplate { get; set; }

		public object CurrentItem { get; set; }

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
					ModeChanging.InvokeAsync(new FormViewModeEventArgs() { NewMode = DefaultMode }).GetAwaiter().GetResult();
					CurrentMode = DefaultMode;
					break;
				case "edit":
					ModeChanging.InvokeAsync(new FormViewModeEventArgs() { NewMode = FormViewMode.Edit }).GetAwaiter().GetResult();
					CurrentMode = FormViewMode.Edit;
					break;
				case "update":
					OnItemUpdating.InvokeAsync(new FormViewUpdateEventArgs("update")).GetAwaiter().GetResult();
					ModeChanging.InvokeAsync(new FormViewModeEventArgs() { NewMode = DefaultMode }).GetAwaiter().GetResult();
					OnItemUpdated.InvokeAsync(new FormViewUpdatedEventArgs(0, null)).GetAwaiter().GetResult();
					CurrentMode = DefaultMode;
					break;

			}

			StateHasChanged();

		}

		#endregion

		#region Custom Events

		[Parameter]
		public EventCallback<FormViewUpdateEventArgs> OnItemUpdating { get; set; }

		[Parameter]
		public EventCallback<FormViewUpdatedEventArgs> OnItemUpdated { get; set; }

		#endregion

	}

}
