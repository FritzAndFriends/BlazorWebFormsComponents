<%@ Page Title="Announcements" Language="C#" AutoEventWireup="true" CodeBehind="Announcements.aspx.cs" Inherits="DepartmentPortal.AnnouncementsPage" %>
<%@ Register Src="~/Controls/PageHeader.ascx" TagName="PageHeader" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Breadcrumb.ascx" TagName="Breadcrumb" TagPrefix="uc" %>
<%@ Register Src="~/Controls/SearchBox.ascx" TagName="SearchBox" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Pager.ascx" TagName="Pager" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Footer.ascx" TagName="Footer" TagPrefix="uc" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <uc:PageHeader ID="PageHeaderControl" runat="server" Title="Announcements" Description="Stay informed with the latest news and updates" />
    
    <uc:Breadcrumb ID="BreadcrumbControl" runat="server" CurrentPage="Announcements" />
    
    <div class="row">
        <div class="col-md-6">
            <uc:SearchBox ID="SearchBoxControl" runat="server" OnSearch="SearchBoxControl_Search" Placeholder="Search announcements..." />
        </div>
    </div>
    
    <local:SectionPanel ID="AnnouncementsSectionPanel" runat="server" Title="Recent Announcements">
        <ContentTemplate>
            <asp:Repeater ID="AnnouncementsRepeater" runat="server">
                <ItemTemplate>
                    <div class="announcement-item">
                        <h3><%# Eval("Title") %></h3>
                        <p class="text-muted">
                            <span class="glyphicon glyphicon-calendar"></span> <%# Eval("PublishDate", "{0:MMMM d, yyyy}") %>
                            by <%# Eval("Author") %>
                        </p>
                        <p><%# Eval("Body") %></p>
                        <asp:HyperLink runat="server" 
                            NavigateUrl='<%# "AnnouncementDetail.aspx?id=" + Eval("Id") %>' 
                            CssClass="btn btn-sm btn-default">
                            Read More <span class="glyphicon glyphicon-chevron-right"></span>
                        </asp:HyperLink>
                        <hr />
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            
            <asp:Panel ID="NoResultsPanel" runat="server" Visible="false" CssClass="alert alert-info">
                No announcements found matching your search.
            </asp:Panel>
        </ContentTemplate>
    </local:SectionPanel>
    
    <uc:Pager ID="PagerControl" runat="server" OnPageChanged="PagerControl_PageChanged" />
    
    <uc:Footer ID="FooterControl" runat="server" />
</asp:Content>
