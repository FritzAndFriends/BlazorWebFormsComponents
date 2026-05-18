using BlazorWebFormsComponents.Interfaces;
using Microsoft.AspNetCore.Components;

namespace BlazorWebFormsComponents
{
	public abstract class BaseColumn<ItemType> : BaseWebFormsComponent, IColumn<ItemType>
	{
		///<inheritdoc/>
		[CascadingParameter(Name = "ColumnCollection")]
		public IColumnCollection<ItemType> ParentColumnsCollection { get; set; }

		///<inheritdoc/>
		[Parameter] public string HeaderText { get; set; }

		[Parameter] public virtual string SortExpression { get; set; }

		/// <summary>
		/// The form naming context for the current row being rendered.
		/// Set by GridViewRow before calling Render/RenderEdit so that
		/// BoundField and other column types can generate row-scoped form field names.
		/// </summary>
		public FormNamingContext CurrentFormNamingContext { get; set; }

		public void Dispose()
		{
			ParentColumnsCollection?.RemoveColumn(this);
		}

		public abstract RenderFragment Render(ItemType item);

		/// <summary>
		/// Renders the column in edit mode. By default falls back to the normal Render method.
		/// </summary>
		public virtual RenderFragment RenderEdit(ItemType item) => Render(item);

		///<inheritdoc/>
		protected override void OnInitialized()
		{
			ParentColumnsCollection?.AddColumn(this);
		}
	}
}
