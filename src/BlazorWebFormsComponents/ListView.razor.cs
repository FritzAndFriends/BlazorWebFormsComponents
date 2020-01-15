using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{

	public partial class ListView<ItemType> : BaseModelBindingComponent<ItemType>
	{

		public ListView()
		{
		}

		#region Templates

		[Parameter]
		public RenderFragment<ItemType> AlternatingItemTemplate { get; set; }

		/// <summary>
		/// Defines the content to render if the data source returns no data.
		/// </summary>
		[Parameter]
		public RenderFragment EmptyDataTemplate { get; set; }

		[Parameter]
		public RenderFragment ItemSeparatorTemplate { get; set; }

		[Parameter]
		public RenderFragment<ItemType> ItemTemplate { get; set; }

		/// <summary>
		/// 🚨🚨 LayoutTemplate is not available.  Please wrap the ListView component with the desired layout 🚨🚨
		/// </summary>
		[Parameter]
		//[Obsolete("The LayoutTemplate child element is not supported in Blazor.  Instead, wrap the ListView component with the desired layout")]
		public RenderFragment<RenderFragment> LayoutTemplate { get; set; }

		/// <summary>
		/// 🚨🚨 LayoutTemplate and the OnLayoutCreated event is not available.  Please wrap the ListView component with the desired layout 🚨🚨
		/// </summary>
		[Parameter]
		[Obsolete("The layout is not managed by this component, therefore this event will not be raised")]
		public EventHandler OnLayoutCreated { get; set; }

		#endregion

		[Parameter] // TODO: Implement
		public InsertItemPosition InsertItemPosition { get; set; }

		[Parameter] // TODO: Implement
		public int SelectedIndex { get; set; }

		/// <summary>
		/// Style is not applied by this control
		/// </summary>
		[Parameter, Obsolete("Style is not applied by this control")]
		public string Style { get; set; }


		[Parameter]
		public RenderFragment ChildContent { get; set; }

		[CascadingParameter(Name = "Host")]
		public BaseWebFormsComponent HostComponent { get; set; }


		protected override void OnInitialized()
		{

			HostComponent = this;

			base.OnInitialized();

		}

	}

}
