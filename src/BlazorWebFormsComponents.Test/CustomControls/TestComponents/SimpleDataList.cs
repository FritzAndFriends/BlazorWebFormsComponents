using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;
using System.Collections;
using System.Collections.Generic;

namespace BlazorWebFormsComponents.Test.CustomControls.TestComponents
{
	public class SimpleDataList : DataBoundWebControl
	{
		private List<string> _items = new();

		protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Ul;

		protected override void PerformDataBinding(IEnumerable data)
		{
			_items.Clear();
			if (data != null)
			{
				foreach (var item in data)
				{
					_items.Add(item?.ToString() ?? string.Empty);
				}
			}
		}

		protected override void RenderContents(HtmlTextWriter writer)
		{
			foreach (var item in _items)
			{
				writer.RenderBeginTag(HtmlTextWriterTag.Li);
				writer.Write(item);
				writer.RenderEndTag();
			}
		}
	}
}
