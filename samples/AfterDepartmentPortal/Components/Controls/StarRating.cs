using System;
using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

namespace AfterDepartmentPortal.Components.Controls
{
    public class StarRating : WebControl
    {
        [Parameter]
        public int Rating { get; set; }

        [Parameter]
        public bool ReadOnly { get; set; } = true;

        [Parameter]
        public string StarColor { get; set; } = "gold";

        [Parameter]
        public string EmptyStarColor { get; set; } = "lightgray";

        protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Span;

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "star-rating");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Color, StarColor);
            base.AddAttributesToRender(writer);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            var rating = Math.Max(1, Math.Min(5, Rating));
            for (var i = 1; i <= 5; i++)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, i <= rating ? "star filled" : "star empty");
                writer.AddAttribute("data-rating", i.ToString());
                if (i <= rating)
                {
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Color, StarColor);
                }
                else
                {
                    writer.AddStyleAttribute(HtmlTextWriterStyle.Color, EmptyStarColor);
                }
                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                writer.Write("★");
                writer.RenderEndTag();
            }
        }
    }
}
