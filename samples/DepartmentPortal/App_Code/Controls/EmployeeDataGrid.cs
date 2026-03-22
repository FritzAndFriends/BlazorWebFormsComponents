using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DepartmentPortal.Controls
{
    public class EmployeeDataGrid : DataBoundControl
    {
        public string SearchText
        {
            get { return (string)(ViewState["SearchText"] ?? string.Empty); }
            set { ViewState["SearchText"] = value; }
        }

        public string SortColumn
        {
            get { return (string)(ViewState["SortColumn"] ?? string.Empty); }
            set { ViewState["SortColumn"] = value; }
        }

        public string SortDirection
        {
            get { return (string)(ViewState["SortDirection"] ?? "ASC"); }
            set { ViewState["SortDirection"] = value; }
        }

        public int PageSize
        {
            get { return (int)(ViewState["PageSize"] ?? 10); }
            set { ViewState["PageSize"] = value; }
        }

        public bool AllowPaging
        {
            get { return (bool)(ViewState["AllowPaging"] ?? false); }
            set { ViewState["AllowPaging"] = value; }
        }

        public bool AllowSorting
        {
            get { return (bool)(ViewState["AllowSorting"] ?? false); }
            set { ViewState["AllowSorting"] = value; }
        }

        public bool AllowSearch
        {
            get { return (bool)(ViewState["AllowSearch"] ?? false); }
            set { ViewState["AllowSearch"] = value; }
        }

        public int CurrentPageIndex
        {
            get { return (int)(ViewState["CurrentPageIndex"] ?? 0); }
            set { ViewState["CurrentPageIndex"] = value; }
        }

        private List<object> dataItems = new List<object>();

        protected override void PerformDataBinding(IEnumerable data)
        {
            base.PerformDataBinding(data);
            
            dataItems.Clear();
            if (data != null)
            {
                foreach (object item in data)
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
                writer.AddAttribute("placeholder", "Search employees...");
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

            int startIndex = AllowPaging ? CurrentPageIndex * PageSize : 0;
            int endIndex = AllowPaging ? Math.Min(startIndex + PageSize, dataItems.Count) : dataItems.Count;

            for (int i = startIndex; i < endIndex; i++)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Tr);
                
                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write(i + 1);
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write("Employee " + (i + 1));
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write("Title " + (i + 1));
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write("Department " + ((i % 3) + 1));
                writer.RenderEndTag();

                writer.RenderBeginTag(HtmlTextWriterTag.Td);
                writer.Write("<a href='#'>View</a> | <a href='#'>Edit</a>");
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
                
                int totalPages = (int)Math.Ceiling((double)dataItems.Count / PageSize);
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
