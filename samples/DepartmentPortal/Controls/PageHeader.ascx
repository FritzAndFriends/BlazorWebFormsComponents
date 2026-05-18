<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PageHeader.ascx.cs" Inherits="DepartmentPortal.Controls.PageHeader" %>

<div class="page-header">
    <h1><asp:Literal ID="litPageTitle" runat="server" /></h1>
    <div class="user-info" runat="server" id="pnlUserInfo" visible="false">
        <span class="welcome-message">Welcome, <asp:Literal ID="litUserName" runat="server" /></span>
    </div>
</div>
