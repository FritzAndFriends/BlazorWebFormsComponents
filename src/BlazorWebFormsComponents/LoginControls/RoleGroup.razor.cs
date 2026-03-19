using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.LoginControls
{
	public partial class RoleGroup : BaseWebFormsComponent
	{
		[Parameter] public string Roles { get; set; }

		/// <summary>
		/// Matches Web Forms &lt;ContentTemplate&gt; syntax inside RoleGroup.
		/// </summary>
		[Parameter] public RenderFragment ContentTemplate { get; set; }

		/// <summary>
		/// Backward-compatible alias — bare child content inside RoleGroup tags.
		/// </summary>
		[Parameter] public RenderFragment ChildContent { get; set; }

		[CascadingParameter(Name = "LoginView")]
		public LoginView LoginView { get; set; }

		protected override void OnParametersSet()
		{
			if (!LoginView.RoleGroupCollection.Contains(this))
			{
				LoginView.RoleGroupCollection.Add(this);
			}
		}
	}
}
