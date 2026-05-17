using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Threading.Tasks;

namespace BlazorWebFormsComponents
{
	/// <summary>
	/// A GridView command column that renders edit/delete/select actions using the parent GridView.
	/// </summary>
	public partial class CommandField<ItemType> : BaseColumn<ItemType>
	{
		[CascadingParameter(Name = "ParentGridView")]
		public GridView<ItemType> ParentGridView { get; set; }

		[Parameter] public string ButtonType { get; set; } = "Link";
		[Parameter] public string CancelText { get; set; } = "Cancel";
		[Parameter] public string DeleteText { get; set; } = "Delete";
		[Parameter] public string EditText { get; set; } = "Edit";
		[Parameter] public bool ShowCancelButton { get; set; } = true;
		[Parameter] public bool ShowDeleteButton { get; set; }
		[Parameter] public bool ShowEditButton { get; set; }
		[Parameter] public bool ShowSelectButton { get; set; }
		[Parameter] public string SelectText { get; set; } = "Select";
		[Parameter] public string UpdateText { get; set; } = "Update";

		public override RenderFragment Render(ItemType item) => builder =>
		{
			var seq = 0;
			var rowIndex = CurrentFormNamingContext?.RowIndex ?? 0;

			if (ShowSelectButton)
			{
				RenderAction(builder, ref seq, SelectText, () => ParentGridView?.SelectRow(rowIndex) ?? Task.CompletedTask);
			}

			if (ShowEditButton)
			{
				RenderSeparatorIfNeeded(builder, ref seq);
				RenderAction(builder, ref seq, EditText, () => ParentGridView?.EditRow(rowIndex) ?? Task.CompletedTask);
			}

			if (ShowDeleteButton)
			{
				RenderSeparatorIfNeeded(builder, ref seq);
				RenderAction(builder, ref seq, DeleteText, () => ParentGridView?.DeleteRow(rowIndex) ?? Task.CompletedTask);
			}
		};

		public override RenderFragment RenderEdit(ItemType item) => builder =>
		{
			var seq = 0;
			var rowIndex = CurrentFormNamingContext?.RowIndex ?? 0;

			if (ShowEditButton)
			{
				RenderAction(builder, ref seq, UpdateText, () => ParentGridView?.UpdateRow(rowIndex) ?? Task.CompletedTask);

				if (ShowCancelButton)
				{
					RenderSeparatorIfNeeded(builder, ref seq);
					RenderAction(builder, ref seq, CancelText, () => ParentGridView?.CancelEdit(rowIndex) ?? Task.CompletedTask);
				}
			}
			else
			{
				Render(item)(builder);
			}
		};

		private void RenderAction(RenderTreeBuilder builder, ref int seq, string text, Func<Task> callback)
		{
			if (UsesButtonTag)
			{
				builder.OpenElement(seq++, "button");
				builder.AddAttribute(seq++, "type", "button");
				builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create(this, callback));
				builder.AddContent(seq++, text);
				builder.CloseElement();
				return;
			}

			builder.OpenElement(seq++, "a");
			builder.AddAttribute(seq++, "href", "javascript:void(0);");
			builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create(this, callback));
			builder.AddAttribute(seq++, "onclick:preventDefault", true);
			builder.AddContent(seq++, text);
			builder.CloseElement();
		}

		private static void RenderSeparatorIfNeeded(RenderTreeBuilder builder, ref int seq)
		{
			if (seq > 0)
			{
				builder.AddMarkupContent(seq++, "&nbsp;");
			}
		}

		private bool UsesButtonTag => string.Equals(ButtonType, "Button", StringComparison.OrdinalIgnoreCase);
	}
}
