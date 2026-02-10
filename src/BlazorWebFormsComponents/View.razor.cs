using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public partial class View : BaseWebFormsComponent
	{
		[CascadingParameter(Name = "ParentMultiView")]
		public MultiView ParentMultiView { get; set; }

		[Parameter]
		public RenderFragment ChildContent { get; set; }

		[Parameter]
		public EventCallback<EventArgs> OnActivate { get; set; }

		[Parameter]
		public EventCallback<EventArgs> OnDeactivate { get; set; }

		protected override async Task OnInitializedAsync()
		{
			await base.OnInitializedAsync();
			ParentMultiView?.RegisterView(this);
		}

		internal void NotifyActivated()
		{
			OnActivate.InvokeAsync(EventArgs.Empty);
		}

		internal void NotifyDeactivated()
		{
			OnDeactivate.InvokeAsync(EventArgs.Empty);
		}
	}
}
