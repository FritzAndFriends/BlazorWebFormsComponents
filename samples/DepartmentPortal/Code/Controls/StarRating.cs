using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace DepartmentPortal.Controls
{
    public class StarRating : WebControl
    {
        public int Rating
        {
            get { return (int)(ViewState["Rating"] ?? 0); }
            set { ViewState["Rating"] = Math.Max(1, Math.Min(5, value)); }
        }

        public bool ReadOnly
        {
            get { return (bool)(ViewState["ReadOnly"] ?? true); }
            set { ViewState["ReadOnly"] = value; }
        }

        public string StarColor
        {
            get { return (string)(ViewState["StarColor"] ?? "gold"); }
            set { ViewState["StarColor"] = value; }
        }

        public string EmptyStarColor
        {
            get { return (string)(ViewState["EmptyStarColor"] ?? "lightgray"); }
            set { ViewState["EmptyStarColor"] = value; }
        }

        protected override void AddAttributesToRender(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "star-rating");
            writer.AddStyleAttribute(HtmlTextWriterStyle.Color, StarColor);
            base.AddAttributesToRender(writer);
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            int rating = Rating;
            for (int i = 1; i <= 5; i++)
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

        protected override HtmlTextWriterTag TagKey
        {
            get { return HtmlTextWriterTag.Span; }
        }
    }
}
