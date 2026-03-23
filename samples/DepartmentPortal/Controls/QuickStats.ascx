<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="QuickStats.ascx.cs" Inherits="DepartmentPortal.Controls.QuickStats" %>

<div class="quick-stats">
    <div class="stat-item" runat="server" id="pnlEmployeeCount" visible="false">
        <span class="stat-label">Employees</span>
        <span class="stat-value"><asp:Literal ID="litEmployeeCount" runat="server" /></span>
    </div>
    <div class="stat-item" runat="server" id="pnlAnnouncementCount" visible="false">
        <span class="stat-label">Announcements</span>
        <span class="stat-value"><asp:Literal ID="litAnnouncementCount" runat="server" /></span>
    </div>
</div>
