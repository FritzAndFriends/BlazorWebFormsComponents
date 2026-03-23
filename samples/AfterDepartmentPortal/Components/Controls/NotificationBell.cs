using System;
using BlazorWebFormsComponents.CustomControls;
using Microsoft.AspNetCore.Components;

namespace AfterDepartmentPortal.Components.Controls
{
    public class NotificationBell : WebControl
    {
        [Parameter]
        public int UnreadCount { get; set; }

        [Parameter]
        public int MaxNotifications { get; set; } = 5;

        [Parameter]
        public bool DrawerVisible { get; set; }

        [Parameter]
        public EventCallback<NotificationEventArgs> NotificationClicked { get; set; }

        [Parameter]
        public EventCallback<NotificationEventArgs> NotificationDismissed { get; set; }

        protected override HtmlTextWriterTag TagKey => HtmlTextWriterTag.Div;

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

                var displayCount = Math.Min(UnreadCount, MaxNotifications);
                for (var i = 0; i < displayCount; i++)
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
    }
}
