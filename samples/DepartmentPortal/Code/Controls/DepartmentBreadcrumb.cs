using System;
using System.Web.UI;
using DepartmentPortal.Models;

namespace DepartmentPortal.Controls
{
    public class DepartmentBreadcrumb : Control, IPostBackEventHandler
    {
        public string OrganizationName
        {
            get { return (string)(ViewState["OrganizationName"] ?? string.Empty); }
            set { ViewState["OrganizationName"] = value; }
        }

        public string DivisionName
        {
            get { return (string)(ViewState["DivisionName"] ?? string.Empty); }
            set { ViewState["DivisionName"] = value; }
        }

        public string DepartmentName
        {
            get { return (string)(ViewState["DepartmentName"] ?? string.Empty); }
            set { ViewState["DepartmentName"] = value; }
        }

        public int DepartmentId
        {
            get { return (int)(ViewState["DepartmentId"] ?? 0); }
            set { ViewState["DepartmentId"] = value; }
        }

        public string Separator
        {
            get { return (string)(ViewState["Separator"] ?? " → "); }
            set { ViewState["Separator"] = value; }
        }

        public bool EnableLinks
        {
            get { return (bool)(ViewState["EnableLinks"] ?? true); }
            set { ViewState["EnableLinks"] = value; }
        }

        public string LinkCssClass
        {
            get { return (string)(ViewState["LinkCssClass"] ?? "breadcrumb-link"); }
            set { ViewState["LinkCssClass"] = value; }
        }

        public event EventHandler<BreadcrumbEventArgs> BreadcrumbItemClicked;

        protected virtual void OnBreadcrumbItemClicked(BreadcrumbEventArgs e)
        {
            if (BreadcrumbItemClicked != null)
            {
                BreadcrumbItemClicked(this, e);
            }
        }

        public void RaisePostBackEvent(string eventArgument)
        {
            if (!string.IsNullOrEmpty(eventArgument))
            {
                string[] parts = eventArgument.Split('|');
                if (parts.Length == 2)
                {
                    OnBreadcrumbItemClicked(new BreadcrumbEventArgs
                    {
                        DepartmentId = DepartmentId,
                        ItemName = parts[0],
                        NavigationLevel = parts[1]
                    });
                }
            }
        }

        protected override void Render(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "department-breadcrumb");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            bool isFirst = true;

            if (!string.IsNullOrEmpty(OrganizationName))
            {
                RenderBreadcrumbItem(writer, OrganizationName, "organization", isFirst);
                isFirst = false;
            }

            if (!string.IsNullOrEmpty(DivisionName))
            {
                if (!isFirst) writer.Write(System.Web.HttpUtility.HtmlEncode(Separator));
                RenderBreadcrumbItem(writer, DivisionName, "division", isFirst);
                isFirst = false;
            }

            if (!string.IsNullOrEmpty(DepartmentName))
            {
                if (!isFirst) writer.Write(System.Web.HttpUtility.HtmlEncode(Separator));
                RenderBreadcrumbItem(writer, DepartmentName, "department", isFirst);
            }

            writer.RenderEndTag(); // div
        }

        private void RenderBreadcrumbItem(HtmlTextWriter writer, string text, string level, bool isFirst)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "breadcrumb-item");
            writer.RenderBeginTag(HtmlTextWriterTag.Span);

            if (EnableLinks)
            {
                string postBackScript = Page.ClientScript.GetPostBackEventReference(this, text + "|" + level);
                writer.AddAttribute(HtmlTextWriterAttribute.Href, "javascript:" + postBackScript);
                writer.AddAttribute(HtmlTextWriterAttribute.Class, LinkCssClass);
                writer.RenderBeginTag(HtmlTextWriterTag.A);
                writer.Write(System.Web.HttpUtility.HtmlEncode(text));
                writer.RenderEndTag(); // a
            }
            else
            {
                writer.Write(System.Web.HttpUtility.HtmlEncode(text));
            }

            writer.RenderEndTag(); // span
        }
    }
}
