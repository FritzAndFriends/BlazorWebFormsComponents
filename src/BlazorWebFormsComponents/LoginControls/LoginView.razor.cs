using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorWebFormsComponents.LoginControls
{
  public partial class LoginView : BaseWebFormsComponent
  {
	[Inject]
	protected AuthenticationStateProvider AuthenticationStateProvider { get; set; }

	[Parameter] public RoleGroupCollection RoleGroups { get; set; } = new RoleGroupCollection();

	[Parameter] public RenderFragment LoggedInTemplate { get; set; }
	[Parameter] public RenderFragment AnonymousTemplate { get; set; }
	[Parameter] public RenderFragment ChildContent { get; set; }


	private RenderFragment GetView()
	{

	  if (!(_user.Identity?.IsAuthenticated ?? false))
	  {

			return AnonymousTemplate;

	  }
	  else
	  {

			var roleGroup = RoleGroups.GetRoleGroup(_user);
			if (roleGroup != null)
			{

			  return roleGroup.ChildContent;

			}
			else
			{

				return LoggedInTemplate;

			}

	  }

	}

	private ClaimsPrincipal _user;



	protected override async Task OnInitializedAsync()
	{

	  var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();

	  _user = authState.User;

	  await base.OnInitializedAsync();

	}

  }
}
