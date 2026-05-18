using System;
using System.Net;
using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

namespace AfterDepartmentPortal.Components.Controls
{
    public class DepartmentBreadcrumb : WebControl
    {
        [Parameter]
        public string OrganizationName { get; set; } = string.Empty;

        [Parameter]
        public string DivisionName { get; set; } = string.Empty;

        [Parameter]
        public string DepartmentName { get; set; } = string.Empty;

        [Parameter]
        public int DepartmentId { get; set; }

        [Parameter]
        public string Separator { get; set; } = " → ";

        [Parameter]
        public bool EnableLinks { get; set; } = true;

        [Parameter]
        public string LinkCssClass { get; set; } = "breadcrumb-link";

        [Parameter]
        public EventCallback<BreadcrumbEventArgs> BreadcrumbItemClicked { get; set; }

        protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "department-breadcrumb");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            var isFirst = true;

            if (!string.IsNullOrEmpty(OrganizationName))
            {
                RenderBreadcrumbItem(writer, OrganizationName, "organization");
                isFirst = false;
            }

            if (!string.IsNullOrEmpty(DivisionName))
            {
                if (!isFirst) writer.Write(WebUtility.HtmlEncode(Separator));
                RenderBreadcrumbItem(writer, DivisionName, "division");
                isFirst = false;
            }

            if (!string.IsNullOrEmpty(DepartmentName))
            {
                if (!isFirst) writer.Write(WebUtility.HtmlEncode(Separator));
                RenderBreadcrumbItem(writer, DepartmentName, "department");
            }

            writer.RenderEndTag(); // div
        }

        private void RenderBreadcrumbItem(HtmlTextWriter writer, string text, string level)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "breadcrumb-item");
            writer.RenderBeginTag(HtmlTextWriterTag.Span);

            if (EnableLinks)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
                writer.AddAttribute(HtmlTextWriterAttribute.Class, LinkCssClass);
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write(WebUtility.HtmlEncode(text));
                writer.RenderEndTag(); // a
            }
            else
            {
                writer.Write(WebUtility.HtmlEncode(text));
            }

            writer.RenderEndTag(); // span
        }
    }
}
