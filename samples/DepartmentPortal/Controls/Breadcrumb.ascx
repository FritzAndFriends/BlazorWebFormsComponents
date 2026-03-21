<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Breadcrumb.ascx.cs" Inherits="DepartmentPortal.Controls.Breadcrumb" %>

<nav aria-label="breadcrumb">
    <ol class="breadcrumb">
        <li class="breadcrumb-item" runat="server" id="homeLinkItem" visible="false">
            <a href="/">Home</a>
        </li>
        <asp:Repeater ID="rptBreadcrumb" runat="server">
            <ItemTemplate>
                <li class='<%# Container.ItemIndex == ((System.Collections.IList)((System.Web.UI.WebControls.Repeater)Container.NamingContainer).DataSource).Count - 1 ? "breadcrumb-item active" : "breadcrumb-item" %>'
                    aria-current='<%# Container.ItemIndex == ((System.Collections.IList)((System.Web.UI.WebControls.Repeater)Container.NamingContainer).DataSource).Count - 1 ? "page" : "" %>'>
                    <%# Container.DataItem %>
                </li>
            </ItemTemplate>
        </asp:Repeater>
    </ol>
</nav>
