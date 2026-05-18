<%@ Page Title="Announcement" Language="C#" AutoEventWireup="true" CodeBehind="AnnouncementDetail.aspx.cs" Inherits="DepartmentPortal.AnnouncementDetailPage" %>
<%@ Register Src="~/Controls/PageHeader.ascx" TagName="PageHeader" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Breadcrumb.ascx" TagName="Breadcrumb" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Footer.ascx" TagName="Footer" TagPrefix="uc" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <uc:PageHeader ID="PageHeaderControl" runat="server" />
    
    <uc:Breadcrumb ID="BreadcrumbControl" runat="server" CurrentPage="Announcement Details" />
    
    <asp:Panel ID="AnnouncementDetailsPanel" runat="server" Visible="false">
        <div class="announcement-detail">
            <div class="panel panel-default">
                <div class="panel-body">
                    <h2><asp:Label ID="TitleLabel" runat="server" /></h2>
                    <p class="text-muted">
                        <span class="glyphicon glyphicon-calendar"></span> 
                        <asp:Label ID="PublishDateLabel" runat="server" />
                        by <asp:Label ID="AuthorLabel" runat="server" />
                    </p>
                    <hr />
                    <div class="announcement-body">
                        <asp:Label ID="BodyLabel" runat="server" />
                    </div>
                </div>
            </div>
            
            <asp:HyperLink runat="server" NavigateUrl="~/Announcements.aspx" CssClass="btn btn-default">
                <span class="glyphicon glyphicon-arrow-left"></span> Back to Announcements
            </asp:HyperLink>
        </div>
    </asp:Panel>
    
    <asp:Panel ID="NotFoundPanel" runat="server" Visible="false">
        <div class="alert alert-warning">
            <strong>Announcement not found.</strong> The announcement you are looking for does not exist or has been removed.
        </div>
        <asp:HyperLink runat="server" NavigateUrl="~/Announcements.aspx" CssClass="btn btn-default">
            <span class="glyphicon glyphicon-arrow-left"></span> Back to Announcements
        </asp:HyperLink>
    </asp:Panel>
    
    <uc:Footer ID="FooterControl" runat="server" />
</asp:Content>
