using System;
using System.Net;
using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

namespace AfterDepartmentPortal.Components.Controls
{
    public class SectionPanel : TemplatedWebControl
    {
        [Parameter]
        public string Title { get; set; } = string.Empty;

        [Parameter]
        public new string CssClass { get; set; } = "section-panel";

        [Parameter]
        public RenderFragment? HeaderTemplate { get; set; }

        [Parameter]
        public RenderFragment? ContentTemplate { get; set; }

        [Parameter]
        public RenderFragment? FooterTemplate { get; set; }

        protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            // Header section
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "section-header");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            if (HeaderTemplate != null)
            {
                RenderTemplate(writer, HeaderTemplate);
            }
            else if (!string.IsNullOrEmpty(Title))
            {
                writer.Write($"<h3>{WebUtility.HtmlEncode(Title)}</h3>");
            }
            writer.RenderEndTag(); // section-header div

            // Content section
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "section-content");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            if (ContentTemplate != null)
            {
                RenderTemplate(writer, ContentTemplate);
            }
            writer.RenderEndTag(); // section-content div

            // Footer section (only if template provided)
            if (FooterTemplate != null)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "section-footer");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                RenderTemplate(writer, FooterTemplate);
                writer.RenderEndTag(); // section-footer div
            }

            writer.RenderEndTag(); // main panel div
        }
    }
}
