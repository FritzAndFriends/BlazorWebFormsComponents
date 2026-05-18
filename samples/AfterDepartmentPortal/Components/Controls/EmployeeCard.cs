using System;
using System.Net;
using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

namespace AfterDepartmentPortal.Components.Controls
{
    public class EmployeeCard : WebControl
    {
        [Parameter]
        public int EmployeeId { get; set; }

        [Parameter]
        public string EmployeeName { get; set; } = string.Empty;

        [Parameter]
        public string Title { get; set; } = string.Empty;

        [Parameter]
        public string Department { get; set; } = string.Empty;

        [Parameter]
        public string PhotoUrl { get; set; } = string.Empty;

        [Parameter]
        public bool ShowContactInfo { get; set; }

        [Parameter]
        public bool EnableDetailsLink { get; set; }

        protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Span;

        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "employee-card");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            if (!string.IsNullOrEmpty(PhotoUrl))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Src, PhotoUrl);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "employee-photo");
                writer.AddAttribute(HtmlTextWriterAttribute.Alt, WebUtility.HtmlEncode(EmployeeName));
                writer.RenderBeginTag(HtmlTextWriterTag.Img);
                writer.RenderEndTag();
            }

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "employee-info");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "employee-name");
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(WebUtility.HtmlEncode(EmployeeName));
            writer.RenderEndTag();

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "employee-title");
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(WebUtility.HtmlEncode(Title));
            writer.RenderEndTag();

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "employee-department");
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write(WebUtility.HtmlEncode(Department));
            writer.RenderEndTag();

            if (ShowContactInfo)
            {
                writer.Write("<div class='employee-contact'>Contact info available</div>");
            }

            writer.RenderEndTag(); // employee-info div

            if (EnableDetailsLink)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Href, $"/EmployeeDetail?id={EmployeeId}");
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "employee-details-link");
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write("View Details");
                writer.RenderEndTag();
            }

            writer.RenderEndTag(); // employee-card div
        }
    }
}
