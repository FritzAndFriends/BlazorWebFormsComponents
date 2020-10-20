using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents.LoginControls
{
	public partial class RoleGroup : BaseWebFormsComponent
	{
		[Parameter] public string Roles { get; set; }
		[Parameter] public RenderFragment ChildContent { get; set; }



		[CascadingParameter(Name = "LoginView")]
		public LoginView LoginView { get; set; }


		protected override void OnParametersSet()
		{

			if (!LoginView.RoleGroups.Contains(this))
			{

				LoginView.RoleGroups.Add(this);

			}

		}

	}
}
