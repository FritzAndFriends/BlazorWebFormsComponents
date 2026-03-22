using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using DepartmentPortal.Models;

namespace DepartmentPortal.Controls
{
    public class NotificationBell : WebControl
    {
        public int UnreadCount
        {
            get { return (int)(ViewState["UnreadCount"] ?? 0); }
            set { ViewState["UnreadCount"] = value; }
        }

        public int MaxNotifications
        {
            get { return (int)(ViewState["MaxNotifications"] ?? 5); }
            set { ViewState["MaxNotifications"] = value; }
        }

        public bool DrawerVisible
        {
            get { return (bool)(ViewState["DrawerVisible"] ?? false); }
            set { ViewState["DrawerVisible"] = value; }
        }

        public event EventHandler<NotificationEventArgs> NotificationClicked;
        public event EventHandler<NotificationEventArgs> NotificationDismissed;

        protected virtual void OnNotificationClicked(NotificationEventArgs e)
        {
            if (NotificationClicked != null)
            {
                NotificationClicked(this, e);
            }
        }

        protected virtual void OnNotificationDismissed(NotificationEventArgs e)
        {
            if (NotificationDismissed != null)
            {
                NotificationDismissed(this, e);
            }
        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            writer.AddAttribute(HtmlTextWriterAttribute.Class, "notification-bell-container");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.AddAttribute(HtmlTextWriterAttribute.Class, "notification-bell-icon");
            writer.RenderBeginTag(HtmlTextWriterTag.Span);
            writer.Write("🔔");
            
            if (UnreadCount > 0)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "notification-badge");
                writer.RenderBeginTag(HtmlTextWriterTag.Span);
                writer.Write(UnreadCount > 99 ? "99+" : UnreadCount.ToString());
                writer.RenderEndTag();
            }
            
            writer.RenderEndTag(); // bell-icon span

            if (DrawerVisible)
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, "notification-drawer");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "notification-drawer-header");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);
                writer.Write("Notifications");
                writer.RenderEndTag();

                writer.AddAttribute(HtmlTextWriterAttribute.Class, "notification-drawer-content");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                int displayCount = Math.Min(UnreadCount, MaxNotifications);
                for (int i = 0; i < displayCount; i++)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "notification-item");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    writer.Write("Sample notification " + (i + 1));
                    writer.RenderEndTag();
                }

                if (UnreadCount == 0)
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "no-notifications");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);
                    writer.Write("No new notifications");
                    writer.RenderEndTag();
                }

                writer.RenderEndTag(); // drawer-content div
                writer.RenderEndTag(); // drawer div
            }

            writer.RenderEndTag(); // container div
        }

        protected override HtmlTextWriterTag TagKey
        {
            get { return HtmlTextWriterTag.Div; }
        }
    }
}
