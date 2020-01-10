using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BlazorWebFormsComponents
{
	public class InnerTemplate : ComponentBase
	{

		[Parameter]
		public RenderFragment ChildContent { get; set; }

		[CascadingParameter(Name ="Host")]
		public BaseWebFormsComponent TheComponentToRender { get; set; }

		protected string PlaceHolderID { get; set; }

		protected override void BuildRenderTree(RenderTreeBuilder builder)
		{

			base.BuildRenderTree(builder);
		}

		protected override void OnInitialized()
		{

			//

			base.OnInitialized();
		}

		protected RenderFragment RenderContent(ComponentBase instance)

		{

			TheComponentToRender.LayoutTemplateRendered = true;
			var fragmentField = GetPrivateField(instance.GetType(), "_renderFragment");

			var value = (RenderFragment)fragmentField.GetValue(instance);

			return value;

		}


		private static FieldInfo GetPrivateField(Type t, String name)

		{

			const BindingFlags bf = BindingFlags.Instance |

															BindingFlags.NonPublic |

															BindingFlags.DeclaredOnly;

			FieldInfo fi;

			while ((fi = t.GetField(name, bf)) == null && (t = t.BaseType) != null) ;

			return fi;

		}

	}

	public partial class LayoutTemplate {

		[Parameter]
		public string ItemPlaceholderID
		{
			get { return PlaceHolderID; }
			set { PlaceHolderID = value; }
		}

	}

}
