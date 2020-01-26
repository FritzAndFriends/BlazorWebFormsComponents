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
	public partial class TreeView : BaseWebFormsComponent, IHasStyle
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
		public EventHandler<TreeNodeEventArgs> OnTreeNodeExpanded { get; set; }

		[Parameter]
		public EventHandler<TreeNodeEventArgs> OnTreeNodeCollapsed { get; set; }

		[Parameter]
		public EventHandler<TreeNodeEventArgs> OnTreeNodeCheckChanged { get; set; }

		#endregion

		#region DataBinding

		public RenderFragment ChildNodesRenderFragment { get; set; }

		private new Task DataBind()
		{

			if (DataSource is XmlDocument) return DataBindXml(DataSource as XmlDocument);

			return Task.CompletedTask;

		}

		private Task DataBindXml(XmlDocument src)
		{

			var treeNodeCounter = 0;
			var elements = src.SelectNodes("/*");

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

						builder.AddAttribute(treeNodeCounter++, "Text", element.GetAttribute(thisBinding.TextField));

						if (element.HasChildNodes)
						{
							builder.AddAttribute(treeNodeCounter++, "ChildContent", (Microsoft.AspNetCore.Components.RenderFragment)((_builder) =>
							{

								AddElements(_builder, element.ChildNodes);
							}));
						}
						builder.CloseComponent();
					}
				}

			}

		}

		private List<TreeNodeBinding> _TreeNodeBindings = new List<TreeNodeBinding>();
		internal void AddTreeNodeBinding(TreeNodeBinding treeNodeBinding)
		{
			_TreeNodeBindings.Add(treeNodeBinding);
		}

		#endregion

	}


}



