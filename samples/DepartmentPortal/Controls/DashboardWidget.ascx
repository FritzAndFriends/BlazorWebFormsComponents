<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DashboardWidget.ascx.cs" Inherits="DepartmentPortal.Controls.DashboardWidget" %>

<div class="widget">
    <div class="widget-header">
        <span class="widget-icon"><asp:Literal ID="litIcon" runat="server" /></span>
        <h3 class="widget-title"><asp:Literal ID="litWidgetTitle" runat="server" /></h3>
    </div>
    <div class="widget-body">
        <asp:PlaceHolder ID="phContent" runat="server" />
    </div>
</div>
