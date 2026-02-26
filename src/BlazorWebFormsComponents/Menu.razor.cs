using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using BlazorComponentUtilities;
using BlazorWebFormsComponents.Enums;
using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.JSInterop;

namespace BlazorWebFormsComponents
{

	/// <summary>
	/// A menubar component capable of making hierarchical menus for navigating your application
	/// </summary>
	public partial class Menu : BaseStyledComponent, IMenuStyleContainer
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
		public Orientation Orientation { get; set; } = Orientation.Vertical;

		/// <summary>
		/// Gets or sets how the menu renders HTML. List uses ul/li (default), Table uses table/tr/td (legacy).
		/// </summary>
		[Parameter]
		public MenuRenderingMode RenderingMode { get; set; } = MenuRenderingMode.List;

		[Parameter]
		public int StaticDisplayLevels { get; set; }

		[Parameter]
		public int StaticSubmenuIndent { get; set; }

		[Parameter]
		public Items Items { get; set; }

		#region WI-23: Core missing properties

		/// <summary>
		/// Gets or sets the maximum number of dynamic menu levels to display.
		/// </summary>
		[Parameter]
		public int MaximumDynamicDisplayLevels { get; set; } = 3;

		/// <summary>
		/// Gets or sets the default target window or frame for menu item links.
		/// </summary>
		[Parameter]
		public string Target { get; set; }

		/// <summary>
		/// Gets or sets the text for the accessibility skip link rendered before the menu.
		/// </summary>
		[Parameter]
		public string SkipLinkText { get; set; } = "Skip Navigation Links";

		/// <summary>
		/// Gets or sets the character used to delimit the path of a menu item's value.
		/// </summary>
		[Parameter]
		public char PathSeparator { get; set; } = '/';

		#endregion

		#region Level Styles

		/// <summary>
		/// A collection of MenuLevelStyle objects where index 0 = level 1 styles, index 1 = level 2 styles, etc.
		/// Overrides StaticMenuItemStyle/DynamicMenuItemStyle for items at specific depth levels.
		/// </summary>
		[Parameter]
		public List<MenuLevelStyle> LevelMenuItemStyles { get; set; }

		/// <summary>
		/// A collection of MenuLevelStyle objects for selected items at each depth level.
		/// </summary>
		[Parameter]
		public List<MenuLevelStyle> LevelSelectedStyles { get; set; }

		/// <summary>
		/// A collection of MenuLevelStyle objects for submenu containers at each depth level.
		/// </summary>
		[Parameter]
		public List<MenuLevelStyle> LevelSubMenuStyles { get; set; }

		/// <summary>
		/// Gets the level menu item style for a given depth, or null if none defined.
		/// </summary>
		internal MenuLevelStyle GetLevelMenuItemStyle(int depth)
		{
			var index = depth - 1;
			if (LevelMenuItemStyles != null && index >= 0 && index < LevelMenuItemStyles.Count)
				return LevelMenuItemStyles[index];
			return null;
		}

		/// <summary>
		/// Gets the level selected style for a given depth, or null if none defined.
		/// </summary>
		internal MenuLevelStyle GetLevelSelectedStyle(int depth)
		{
			var index = depth - 1;
			if (LevelSelectedStyles != null && index >= 0 && index < LevelSelectedStyles.Count)
				return LevelSelectedStyles[index];
			return null;
		}

		/// <summary>
		/// Gets the level submenu style for a given depth, or null if none defined.
		/// </summary>
		internal MenuLevelStyle GetLevelSubMenuStyle(int depth)
		{
			var index = depth - 1;
			if (LevelSubMenuStyles != null && index >= 0 && index < LevelSubMenuStyles.Count)
				return LevelSubMenuStyles[index];
			return null;
		}

		#endregion

		#region WI-21: Selection tracking and events

		/// <summary>
		/// Gets the currently selected menu item.
		/// </summary>
		public MenuItem SelectedItem { get; private set; }

		/// <summary>
		/// Gets the value of the currently selected menu item.
		/// </summary>
		public string SelectedValue => SelectedItem?.Value;

		/// <summary>
		/// Fires when a menu item is clicked.
		/// </summary>
		[Parameter]
		public EventCallback<MenuEventArgs> MenuItemClick { get; set; }

		/// <summary>
		/// Fires after each MenuItem is data-bound.
		/// </summary>
		[Parameter]
		public EventCallback<MenuEventArgs> MenuItemDataBound { get; set; }

		/// <summary>
		/// Called by MenuItem when it is clicked.
		/// </summary>
		internal async Task NotifyItemClicked(MenuItem item)
		{
			SelectedItem = item;
			if (MenuItemClick.HasDelegate)
			{
				await MenuItemClick.InvokeAsync(new MenuEventArgs(item));
			}
			StateHasChanged();
		}

		#endregion

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

		private StaticMenuStyle _StaticMenuStyle = new StaticMenuStyle();
		public StaticMenuStyle StaticMenuStyle
		{
			get { return _StaticMenuStyle; }
			set
			{
				_StaticMenuStyle = value;
				this.StateHasChanged();
			}
		}

		#region IMenuStyleContainer explicit implementation

		MenuItemStyle IMenuStyleContainer.DynamicMenuStyle
		{
			get => _DynamicMenuStyle;
			set => DynamicMenuStyle = (DynamicMenuStyle)value;
		}

		MenuItemStyle IMenuStyleContainer.StaticMenuStyle
		{
			get => _StaticMenuStyle;
			set => StaticMenuStyle = (StaticMenuStyle)value;
		}

		MenuItemStyle IMenuStyleContainer.DynamicMenuItemStyle
		{
			get => _DynamicMenuItemStyle;
			set => DynamicMenuItemStyle = (DynamicMenuItemStyle)value;
		}

		MenuItemStyle IMenuStyleContainer.StaticMenuItemStyle
		{
			get => _StaticMenuItemStyle;
			set => StaticMenuItemStyle = (StaticMenuItemStyle)value;
		}

		#endregion

		#region Style RenderFragment Parameters

		/// <summary>
		/// Content for the DynamicMenuStyle sub-component.
		/// </summary>
		[Parameter] public RenderFragment DynamicMenuStyleContent { get; set; }

		/// <summary>
		/// Content for the StaticMenuStyle sub-component.
		/// </summary>
		[Parameter] public RenderFragment StaticMenuStyleContent { get; set; }

		/// <summary>
		/// Content for the DynamicMenuItemStyle sub-component.
		/// </summary>
		[Parameter] public RenderFragment DynamicMenuItemStyleContent { get; set; }

		/// <summary>
		/// Content for the StaticMenuItemStyle sub-component.
		/// </summary>
		[Parameter] public RenderFragment StaticMenuItemStyleContent { get; set; }

		#endregion


		[Parameter]
		public RenderFragment ChildContent { get; set; }

		protected override void OnParametersSet()
		{
			base.OnParametersSet();

			// Auto-generate an ID if none was provided, ensuring JS interop has a valid element ID
			if (string.IsNullOrEmpty(ID))
			{
				ID = $"menu_{GetHashCode():x}";
			}
		}

		protected override async Task OnAfterRenderAsync(bool firstRender) {

			await base.OnAfterRenderAsync(firstRender);

			if (firstRender && DataSource != null) await DataBind();

			if (firstRender)
			{
				await JS.InvokeVoidAsync("bwfc.Page.AddScriptElement", $"{StaticFilesLocation}Menu/Menu.js", $"new Sys.WebForms.Menu({{ element: '{ID}', disappearAfter: {DisappearAfter}, orientation: '{Orientation.ToString().ToLower()}', tabIndex: 0, disabled: false }});");
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

						// Fire MenuItemDataBound after each data-bound item
						if (MenuItemDataBound.HasDelegate)
						{
							_ = MenuItemDataBound.InvokeAsync(new MenuEventArgs(null));
						}

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
