using Microsoft.AspNetCore.Components;
using System;
using System.Security.Claims;

namespace BlazorWebFormsComponents.LoginControls
{
	public partial class LoginView : BaseWebFormsComponent
	{
		[Parameter] public RenderFragment AnonymousTemplate { get; set; }
		[Parameter] public RenderFragment LoggedInTemplate { get; set; }

		/// <summary>
		/// Declarative RoleGroup container — renders RoleGroup child components that self-register.
		/// </summary>
		[Parameter] public RenderFragment RoleGroups { get; set; }

		/// <summary>
		/// Internal collection populated by RoleGroup children via cascading parameter.
		/// </summary>
		internal RoleGroupCollection RoleGroupCollection { get; } = new RoleGroupCollection();

		[Parameter] public EventCallback<EventArgs> OnViewChanged { get; set; }
		[Parameter] public EventCallback<EventArgs> OnViewChanging { get; set; }

		private RenderFragment GetAuthenticatedView(ClaimsPrincipal user)
		{

			var roleGroup = RoleGroupCollection.GetRoleGroup(user);
			if (roleGroup != null)
			{
				return roleGroup.ContentTemplate ?? roleGroup.ChildContent;
			}
			return LoggedInTemplate;
		}
	}
}
