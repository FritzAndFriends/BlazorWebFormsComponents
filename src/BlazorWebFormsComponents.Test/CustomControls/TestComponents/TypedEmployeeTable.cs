using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BlazorWebFormsComponents.Test.CustomControls.TestComponents
{
	public class TestEmployee
	{
		public string Name { get; set; }
		public string Department { get; set; }
	}

	public class TypedEmployeeTable : DataBoundWebControl<TestEmployee>
	{
		protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Table;

		protected override void RenderContents(HtmlTextWriter writer)
		{
			foreach (var emp in TypedDataItems)
			{
				writer.RenderBeginTag(HtmlTextWriterTag.Tr);
				writer.RenderBeginTag(HtmlTextWriterTag.Td);
				writer.Write(emp.Name);
				writer.RenderEndTag();
				writer.RenderBeginTag(HtmlTextWriterTag.Td);
				writer.Write(emp.Department);
				writer.RenderEndTag();
				writer.RenderEndTag();
			}
		}
	}
}
