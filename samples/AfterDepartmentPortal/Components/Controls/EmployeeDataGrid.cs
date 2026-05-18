using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;
using AfterDepartmentPortal.Models;

namespace AfterDepartmentPortal.Components.Controls
{
    public class EmployeeDataGrid : DataBoundWebControl
    {
        [Parameter]
        public string SearchText { get; set; } = string.Empty;

        [Parameter]
        public string SortColumn { get; set; } = string.Empty;

        [Parameter]
        public string SortDirection { get; set; } = "ASC";

        [Parameter]
        public int PageSize { get; set; } = 10;

        [Parameter]
        public bool AllowPaging { get; set; }

        [Parameter]
        public bool AllowSorting { get; set; }

        [Parameter]
        public bool AllowSearch { get; set; }

        [Parameter]
        public int CurrentPageIndex { get; set; }

        private readonly List<object> dataItems = new();

        protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

        protected override void PerformDataBinding(IEnumerable data)
        {
            dataItems.Clear();
            if (data != null)
            {
                foreach (var item in data)
                {
                    dataItems.Add(item);
                }
            }
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "employee-data-grid");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            if (AllowSearch)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "grid-toolbar");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                writer.AddAttribute(HtmlTextWriterAttribute.Type, "text");
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "search-box");
                writer.AddAttribute(HtmlTextWriterAttribute.Placeholder, "Search employees...");
                writer.AddAttribute(HtmlTextWriterAttribute.Value, SearchText);
                writer.RenderBeginTag(HtmlTextWriterTag.Input);
                writer.RenderEndTag();

                writer.RenderEndTag(); // toolbar div
            }

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "data-grid-table");
            writer.RenderBeginTag(HtmlTextWriterTag.Table);

            writer.RenderBeginTag(HtmlTextWriterTag.Thead);
            writer.RenderBeginTag(HtmlTextWriterTag.Tr);

            RenderHeaderCell(writer, "ID");
            RenderHeaderCell(writer, "Name");
            RenderHeaderCell(writer, "Title");
            RenderHeaderCell(writer, "Department");
            RenderHeaderCell(writer, "Actions");

            writer.RenderEndTag(); // tr
            writer.RenderEndTag(); // thead

            writer.RenderBeginTag(HtmlTextWriterTag.Tbody);

            var startIndex = AllowPaging ? CurrentPageIndex * PageSize : 0;
            var endIndex = AllowPaging ? Math.Min(startIndex + PageSize, dataItems.Count) : dataItems.Count;

            for (var i = startIndex; i < endIndex; i++)
            {
                var item = dataItems[i];
                var emp = item as Employee;

                writer.RenderBeginTag(HtmlTextWriterTag.Tr);

                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(emp != null ? emp.Id.ToString() : (i + 1).ToString());
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(emp != null ? WebUtility.HtmlEncode(emp.Name) : "Employee " + (i + 1));
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(emp != null ? WebUtility.HtmlEncode(emp.Title) : "Title " + (i + 1));
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(emp != null ? WebUtility.HtmlEncode(emp.Department) : "Department " + ((i % 3) + 1));
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                var viewUrl = emp != null ? "/EmployeeDetail?id=" + emp.Id : "#";
                writer.Write($"<a href='{viewUrl}'>View</a> | <a href='#'>Edit</a>");
                writer.RenderEndTag();

                writer.RenderEndTag(); // tr
            }

            if (dataItems.Count == 0)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                writer.AddAttribute(HtmlTextWriterAttribute.Colspan, "5");
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write("No data available");
                writer.RenderEndTag();
                writer.RenderEndTag();
            }

            writer.RenderEndTag(); // tbody
            writer.RenderEndTag(); // table

            if (AllowPaging && dataItems.Count > PageSize)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "grid-pager");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                var totalPages = (int)Math.Ceiling((double)dataItems.Count / PageSize);
                writer.Write("Page " + (CurrentPageIndex + 1) + " of " + totalPages);

                writer.RenderEndTag(); // pager div
            }

            writer.RenderEndTag(); // grid div
        }

        private void RenderHeaderCell(HtmlTextWriter writer, string text)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "grid-header-cell");
            writer.RenderBeginTag(HtmlTextWriterTag.Th);
            writer.Write(text);
            if (AllowSorting && text != "Actions")
            {
                writer.Write(" ▲▼");
            }
            writer.RenderEndTag();
        }
    }
}
