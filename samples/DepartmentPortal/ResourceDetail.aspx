<%@ Page Title="Resource Details" Language="C#" AutoEventWireup="true" CodeBehind="ResourceDetail.aspx.cs" Inherits="DepartmentPortal.ResourceDetailPage" %>
<%@ Register Src="~/Controls/PageHeader.ascx" TagName="PageHeader" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Breadcrumb.ascx" TagName="Breadcrumb" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Footer.ascx" TagName="Footer" TagPrefix="uc" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <uc:PageHeader ID="PageHeaderControl" runat="server" />
    
    <uc:Breadcrumb ID="BreadcrumbControl" runat="server" CurrentPage="Resource Details" />
    
    <asp:Panel ID="ResourceDetailsPanel" runat="server" Visible="false">
        <div class="resource-detail">
            <div class="panel panel-default">
                <div class="panel-body">
                    <h2><asp:Label ID="TitleLabel" runat="server" /></h2>
                    <p class="text-muted">
                        <span class="glyphicon glyphicon-tag"></span> 
                        Category: <asp:Label ID="CategoryLabel" runat="server" />
                    </p>
                    <hr />
                    <div class="resource-description">
                        <p><asp:Label ID="DescriptionLabel" runat="server" /></p>
                    </div>
                    
                    <div class="resource-meta">
                        <dl class="dl-horizontal">
                            <dt>File Type:</dt>
                            <dd><asp:Label ID="FileTypeLabel" runat="server" /></dd>
                            <dt>Size:</dt>
                            <dd><asp:Label ID="FileSizeLabel" runat="server" /></dd>
                            <dt>Last Updated:</dt>
                            <dd><asp:Label ID="LastUpdatedLabel" runat="server" /></dd>
                        </dl>
                    </div>
                    
                    <div class="resource-actions">
                        <asp:HyperLink ID="DownloadLink" runat="server" CssClass="btn btn-primary">
                            <span class="glyphicon glyphicon-download-alt"></span> Download
                        </asp:HyperLink>
                        <asp:Button ID="ShareButton" runat="server" Text="Share" CssClass="btn btn-default" OnClick="ShareButton_Click" />
                    </div>
                </div>
            </div>
        </div>
    </asp:Panel>
    
    <asp:Panel ID="NotFoundPanel" runat="server" Visible="false">
        <div class="alert alert-warning">
            <strong>Resource not found.</strong> The resource you are looking for does not exist.
        </div>
        <asp:HyperLink runat="server" NavigateUrl="~/Resources.aspx" CssClass="btn btn-default">
            <span class="glyphicon glyphicon-arrow-left"></span> Back to Resource Library
        </asp:HyperLink>
    </asp:Panel>
    
    <asp:HyperLink runat="server" NavigateUrl="~/Resources.aspx" CssClass="btn btn-default">
        <span class="glyphicon glyphicon-arrow-left"></span> Back to Resource Library
    </asp:HyperLink>
    
    <uc:Footer ID="FooterControl" runat="server" />
</asp:Content>
