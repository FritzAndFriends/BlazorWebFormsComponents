using BlazorWebFormsComponents.Enums;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BlazorWebFormsComponents
{
	public partial class TreeView : BaseDataBindingComponent, IHasStyle
	{

		[Parameter]
		public TreeNodeCollection Nodes { get; set; } = new TreeNodeCollection();

		[Parameter]
		public RenderFragment ChildContent { get; set; }

		[Parameter]
		public DataBindings DataBindings { get; set; }

		[Parameter]
		public object DataSource { get; set; }

		[Parameter]
		public TreeViewImageSet ImageSet { get; set; } = TreeViewImageSet.Default;

		[Parameter]
		public TreeNodeTypes ShowCheckBoxes { get; set; }

		[Parameter]
		public bool ShowExpandCollapse { get; set; } = true;

		#region IHasStyle

		[Parameter] public WebColor BackColor { get; set; }
		[Parameter] public WebColor BorderColor { get; set; }
		[Parameter] public BorderStyle BorderStyle { get; set; }
		[Parameter] public Unit BorderWidth { get; set; }
		[Parameter] public string CssClass { get; set; }
		[Parameter] public WebColor ForeColor { get; set; }
		[Parameter] public Unit Height { get; set; }
		[Parameter] public Unit Width { get; set; }
		[Parameter] public bool Font_Bold { get; set; }
		[Parameter] public bool Font_Italic { get; set; }
		[Parameter] public string Font_Names { get; set; }
		[Parameter] public bool Font_Overline { get; set; }
		[Parameter] public FontUnit Font_Size { get; set; }
		[Parameter] public bool Font_Strikeout { get; set; }
		[Parameter] public bool Font_Underline { get; set; }

		#endregion

		#region Events

		protected override async Task OnAfterRenderAsync(bool firstRender)
		{

			await base.OnAfterRenderAsync(firstRender);

			if (firstRender && DataSource != null) await DataBind();

		}

		[Parameter]
		public EventCallback<TreeNodeEventArgs> OnTreeNodeDataBound { get; set; }

		[Parameter]
		public EventCallback<TreeNodeEventArgs> OnTreeNodeCheckChanged { get; set; }

		[Parameter]
		public EventCallback<TreeNodeEventArgs> OnTreeNodeCollapsed { get; set; }

		[Parameter]
		public EventCallback<TreeNodeEventArgs> OnTreeNodeExpanded { get; set; }

		#endregion

		#region DataBinding

		public RenderFragment ChildNodesRenderFragment { get; set; }

		private new Task DataBind()
		{

			OnDataBinding.InvokeAsync(EventArgs.Empty);

			if (DataSource is XmlDocument xmlDoc) {

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

			var treeNodeCounter = 0;

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

					var thisBinding = _TreeNodeBindings.FirstOrDefault(b => b.DataMember == element.LocalName);

					if (thisBinding != null)
					{

						builder.OpenComponent<TreeNode>(treeNodeCounter++);

						if (!string.IsNullOrEmpty(thisBinding.ImageToolTipField))
							builder.AddAttribute(treeNodeCounter++, "ImageToolTip", element.GetAttribute(thisBinding.ImageToolTipField));
						if (!string.IsNullOrEmpty(thisBinding.ImageUrlField))
							builder.AddAttribute(treeNodeCounter++, "ImageUrl", element.GetAttribute(thisBinding.ImageUrlField));
						if (!string.IsNullOrEmpty(thisBinding.NavigateUrlField))
							builder.AddAttribute(treeNodeCounter++, "NavigateUrl", element.GetAttribute(thisBinding.NavigateUrlField));
						if (!string.IsNullOrEmpty(thisBinding.TargetField))
							builder.AddAttribute(treeNodeCounter++, "Target", element.GetAttribute(thisBinding.TargetField));

						// Text must be present, no need to test
						builder.AddAttribute(treeNodeCounter++, "Text", element.GetAttribute(thisBinding.TextField));

						if (!string.IsNullOrEmpty(thisBinding.ToolTipField))
							builder.AddAttribute(treeNodeCounter++, "ToolTip", element.GetAttribute(thisBinding.ToolTipField));


						if (element.HasChildNodes)
						{
							builder.AddAttribute(treeNodeCounter++, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((_builder) =>
							{

								AddElements(_builder, element.ChildNodes);

							}));
						}
						builder.CloseComponent();
						OnTreeNodeDataBound.InvokeAsync(new TreeNodeEventArgs(null));

					}
				}

			}

		}

		private Task DataBindSiteMap(XmlDocument src) {

			_TreeNodeBindings.First().DataMember = "siteMapNode";

			return DataBindXml(src.SelectNodes("/siteMap/siteMapNode"));

		}

		private HashSet<TreeNodeBinding> _TreeNodeBindings = new HashSet<TreeNodeBinding>();
		internal void AddTreeNodeBinding(TreeNodeBinding treeNodeBinding)
		{
			_TreeNodeBindings.Add(treeNodeBinding);
		}

		#endregion

	}


}



