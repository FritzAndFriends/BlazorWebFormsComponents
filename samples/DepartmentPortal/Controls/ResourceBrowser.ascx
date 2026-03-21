<%@ Control Language="C#" AutoEventWireup="true" CodeFile="ResourceBrowser.ascx.cs" Inherits="DepartmentPortal.Controls.ResourceBrowser" %>
<%@ Register Src="~/Controls/SearchBox.ascx" TagPrefix="uc" TagName="SearchBox" %>
<%@ Register Src="~/Controls/Breadcrumb.ascx" TagPrefix="uc" TagName="Breadcrumb" %>

<div class="resource-browser">
    <uc:Breadcrumb ID="ctlBreadcrumb" runat="server" ShowHomeLink="true" />
    <uc:SearchBox ID="ctlSearchBox" runat="server" Placeholder="Search resources..." />

    <div class="resource-categories" runat="server" id="pnlCategories" visible="false">
        <h4>Categories</h4>
        <asp:Repeater ID="rptCategories" runat="server" OnItemCommand="rptCategories_ItemCommand">
            <ItemTemplate>
                <asp:LinkButton ID="lnkCategory" runat="server"
                    Text='<%# Eval("CategoryName") %>'
                    CommandName="SelectCategory"
                    CommandArgument='<%# Eval("CategoryId") %>'
                    CssClass="category-link" />
            </ItemTemplate>
        </asp:Repeater>
    </div>

    <div class="resource-list">
        <asp:Repeater ID="rptResources" runat="server" OnItemCommand="rptResources_ItemCommand">
            <ItemTemplate>
                <div class="resource-item">
                    <asp:LinkButton ID="lnkResource" runat="server"
                        Text='<%# Eval("Title") %>'
                        CommandName="SelectResource"
                        CommandArgument='<%# Eval("Id") %>'
                        CssClass="resource-link" />
                    <span class="resource-type"><%# Eval("FileType") %></span>
                    <p class="resource-desc"><%# Eval("Description") %></p>
                </div>
            </ItemTemplate>
        </asp:Repeater>
    </div>
</div>
