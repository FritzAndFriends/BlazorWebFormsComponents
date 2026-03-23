<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AnnouncementCard.ascx.cs" Inherits="DepartmentPortal.Controls.AnnouncementCard" %>

<div class="announcement-card">
    <div class="announcement-header">
        <h3><asp:Literal ID="litTitle" runat="server" /></h3>
        <span class="announcement-date"><asp:Literal ID="litDate" runat="server" /></span>
    </div>
    <div class="announcement-meta">
        <span class="announcement-author">By <asp:Literal ID="litAuthor" runat="server" /></span>
    </div>
    <div class="announcement-body">
        <asp:Literal ID="litBody" runat="server" />
    </div>
</div>
