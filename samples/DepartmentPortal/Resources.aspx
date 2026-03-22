<%@ Page Title="Resources" Language="C#" AutoEventWireup="true" CodeFile="Resources.aspx.cs" Inherits="DepartmentPortal.ResourcesPage" %>
<%@ Register Src="~/Controls/PageHeader.ascx" TagName="PageHeader" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Breadcrumb.ascx" TagName="Breadcrumb" TagPrefix="uc" %>
<%@ Register Src="~/Controls/ResourceBrowser.ascx" TagName="ResourceBrowser" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Footer.ascx" TagName="Footer" TagPrefix="uc" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <uc:PageHeader ID="PageHeaderControl" runat="server" Title="Resource Library" Description="Access documents, templates, and helpful materials" />
    
    <uc:Breadcrumb ID="BreadcrumbControl" runat="server" CurrentPage="Resources" />
    
    <div class="resource-library">
        <local:SectionPanel ID="DocumentsSection" runat="server" Title="Documents" IsCollapsible="true" IsExpanded="true">
            <ContentTemplate>
                <asp:Repeater ID="DocumentsRepeater" runat="server">
                    <ItemTemplate>
                        <div class="resource-item">
                            <span class="glyphicon glyphicon-file"></span>
                            <asp:HyperLink runat="server" NavigateUrl='<%# "ResourceDetail.aspx?id=" + Eval("Id") %>'>
                                <%# Eval("Title") %>
                            </asp:HyperLink>
                            <span class="text-muted">- <%# Eval("Description") %></span>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </ContentTemplate>
        </local:SectionPanel>
        
        <local:SectionPanel ID="TemplatesSection" runat="server" Title="Templates" IsCollapsible="true" IsExpanded="true">
            <ContentTemplate>
                <asp:Repeater ID="TemplatesRepeater" runat="server">
                    <ItemTemplate>
                        <div class="resource-item">
                            <span class="glyphicon glyphicon-duplicate"></span>
                            <asp:HyperLink runat="server" NavigateUrl='<%# "ResourceDetail.aspx?id=" + Eval("Id") %>'>
                                <%# Eval("Title") %>
                            </asp:HyperLink>
                            <span class="text-muted">- <%# Eval("Description") %></span>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </ContentTemplate>
        </local:SectionPanel>
        
        <local:SectionPanel ID="ToolsSection" runat="server" Title="Tools & Utilities" IsCollapsible="true" IsExpanded="false">
            <ContentTemplate>
                <asp:Repeater ID="ToolsRepeater" runat="server">
                    <ItemTemplate>
                        <div class="resource-item">
                            <span class="glyphicon glyphicon-wrench"></span>
                            <asp:HyperLink runat="server" NavigateUrl='<%# "ResourceDetail.aspx?id=" + Eval("Id") %>'>
                                <%# Eval("Title") %>
                            </asp:HyperLink>
                            <span class="text-muted">- <%# Eval("Description") %></span>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </ContentTemplate>
        </local:SectionPanel>
        
        <div style="margin-top: 20px;">
            <uc:ResourceBrowser ID="ResourceBrowserControl" runat="server" />
        </div>
    </div>
    
    <uc:Footer ID="FooterControl" runat="server" />
</asp:Content>
