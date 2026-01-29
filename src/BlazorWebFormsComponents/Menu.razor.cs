using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using BlazorComponentUtilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace BlazorWebFormsComponents
{

	/// <summary>
	/// A menubar component capable of making hierarchical menus for navigating your application
	/// </summary>
	public partial class Menu : BaseWebFormsComponent
	{

		#region Injected Properties

		[Inject]
		public IJSRuntime JS { get; set; }

		#endregion

		[Parameter]
		public object DataSource { get; set; }

		[Parameter]
		public int DisappearAfter { get; set; }

		[Parameter]
		public int StaticDisplayLevels { get; set; }

		[Parameter]
		public int StaticSubmenuIndent { get; set; }

		[Parameter]
		public Items Items { get; set; }

		private DynamicHoverStyle _DynamicHoverStyle = new DynamicHoverStyle();
		public DynamicHoverStyle DynamicHoverStyle {
			get { return _DynamicHoverStyle; }
			set { _DynamicHoverStyle = value;
				StateHasChanged();
			}
		}

		private DynamicMenuStyle _DynamicMenuStyle = new DynamicMenuStyle();
		public DynamicMenuStyle DynamicMenuStyle
		{
			get { return _DynamicMenuStyle; }
			set
			{
				_DynamicMenuStyle = value;
				this.StateHasChanged();
			}
		}

		private DynamicMenuItemStyle _DynamicMenuItemStyle = new DynamicMenuItemStyle();
		public DynamicMenuItemStyle DynamicMenuItemStyle
		{
			get { return _DynamicMenuItemStyle; }
			set
			{
				_DynamicMenuItemStyle = value;
				this.StateHasChanged();
			}
		}

		private DynamicSelectedStyle _DynamicSelectedStyle = new DynamicSelectedStyle();
		public DynamicSelectedStyle DynamicSelectedStyle
		{
			get { return _DynamicSelectedStyle; }
			set
			{
				_DynamicSelectedStyle = value;
				this.StateHasChanged();
			}
		}

		private StaticHoverStyle _StaticHoverStyle = new StaticHoverStyle();
		public StaticHoverStyle StaticHoverStyle {
			get { return _StaticHoverStyle; }
			set {
				_StaticHoverStyle = value;
				StateHasChanged();
			}
		}

		private StaticMenuItemStyle _StaticMenuItemStyle = new StaticMenuItemStyle();
		public StaticMenuItemStyle StaticMenuItemStyle {
			get { return _StaticMenuItemStyle; }
			set {
				_StaticMenuItemStyle = value;
				StateHasChanged();
			}
		}


		[Parameter]
		public RenderFragment ChildContent { get; set; }

		protected override async Task OnAfterRenderAsync(bool firstRender) {

			await base.OnAfterRenderAsync(firstRender);

			if (firstRender && DataSource != null) await DataBind();

			if (firstRender)
			{
				await JS.InvokeVoidAsync("bwfc.Page.AddScriptElement", $"{StaticFilesLocation}Menu/Menu.js", $"new Sys.WebForms.Menu({{ element: '{ID}', disappearAfter: {DisappearAfter}, orientation: 'vertical', tabIndex: 0, disabled: false }});");
			}

		}

		#region Databinding

		[Parameter]
		public EventCallback<EventArgs> OnDataBound { get; set; }

		public RenderFragment ChildNodesRenderFragment { get; set; }

		private new Task DataBind()
		{

			OnDataBinding.InvokeAsync(EventArgs.Empty);

			if (DataSource is XmlDocument xmlDoc)
			{

				if (xmlDoc.SelectSingleNode("/*").LocalName == "siteMap")
					DataBindSiteMap(xmlDoc);
				else
					DataBindXml((DataSource as XmlDocument).SelectNodes("/*"));

			}

			OnDataBound.InvokeAsync(EventArgs.Empty);

			return Task.CompletedTask;

		}

		private Task DataBindXml(XmlNodeList elements)
		{

			var menuItemCounter = 0;

			ChildNodesRenderFragment = b =>
			{
				b.OpenRegion(1);

				AddElements(b, elements);

				b.CloseRegion();
			};

			StateHasChanged();

			return Task.CompletedTask;

			void AddElements(RenderTreeBuilder builder, XmlNodeList siblings)
			{

				foreach (XmlNode node in siblings)
				{

					if (!(node is XmlElement)) continue;
					var element = node as XmlElement;

					var thisBinding = _MenuItemBindings.FirstOrDefault(b => b.DataMember == element.LocalName);

					if (thisBinding != null)
					{

						builder.OpenComponent<MenuItem>(menuItemCounter++);

						//if (!string.IsNullOrEmpty(thisBinding.ImageUrlField))
						//	builder.AddAttribute(menuItemCounter++, "ImageUrl", element.GetAttribute(thisBinding.ImageUrlField));
						if (!string.IsNullOrEmpty(thisBinding.NavigateUrlField))
							builder.AddAttribute(menuItemCounter++, "NavigateUrl", element.GetAttribute(thisBinding.NavigateUrlField).Replace("~\\","/").Replace("~/","/"));
						//if (!string.IsNullOrEmpty(thisBinding.TargetField))
						//	builder.AddAttribute(menuItemCounter++, "Target", element.GetAttribute(thisBinding.TargetField));

						// Text must be present, no need to test
						builder.AddAttribute(menuItemCounter++, "Text", element.GetAttribute(thisBinding.TextField));

						if (!string.IsNullOrEmpty(thisBinding.ToolTipField))
							builder.AddAttribute(menuItemCounter++, "ToolTip", element.GetAttribute(thisBinding.ToolTipField));


						if (element.HasChildNodes)
						{
							builder.AddAttribute(menuItemCounter++, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((_builder) =>
							{

								AddElements(_builder, element.ChildNodes);

							}));
						}
						builder.CloseComponent();

						// TODO: What is the Menu equivalent of OnTreeNodeDataBound??
						//OnTreeNodeDataBound.InvokeAsync(new TreeNodeEventArgs(null));

					}
				}

			}

		}

		private Task DataBindSiteMap(XmlDocument src)
		{

			_MenuItemBindings.Add(new MenuItemBinding
			{
				DataMember = "siteMapNode",
				ToolTipField = "title",
				NavigateUrlField = "url",
				TextField = "description"
			});

			return DataBindXml(src.SelectNodes("/siteMap/siteMapNode"));

		}

		private HashSet<MenuItemBinding> _MenuItemBindings = new HashSet<MenuItemBinding>();

		internal void AddMenuItemBinding(MenuItemBinding menuItemBinding)
		{
			_MenuItemBindings.Add(menuItemBinding);
		}

		#endregion

	}

}
