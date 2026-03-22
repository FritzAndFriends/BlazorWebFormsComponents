<%@ Page Title="My Training" Language="C#" AutoEventWireup="true" CodeFile="MyTraining.aspx.cs" Inherits="DepartmentPortal.MyTrainingPage" %>
<%@ Register Src="~/Controls/PageHeader.ascx" TagName="PageHeader" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Breadcrumb.ascx" TagName="Breadcrumb" TagPrefix="uc" %>
<%@ Register Src="~/Controls/TrainingCatalog.ascx" TagName="TrainingCatalog" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Footer.ascx" TagName="Footer" TagPrefix="uc" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <uc:PageHeader ID="PageHeaderControl" runat="server" Title="My Training" Description="Your enrolled courses and learning path" />
    
    <uc:Breadcrumb ID="BreadcrumbControl" runat="server" CurrentPage="My Training" />
    
    <div class="row">
        <div class="col-md-12">
            <asp:Panel ID="EnrolledCoursesPanel" runat="server" Visible="false">
                <h3>Enrolled Courses (<asp:Label ID="EnrolledCountLabel" runat="server" />)</h3>
                <uc:TrainingCatalog ID="EnrolledTrainingCatalogControl" runat="server" ShowEnrollButton="false" />
            </asp:Panel>
            
            <asp:Panel ID="NoCoursesPanel" runat="server" Visible="false">
                <div class="alert alert-info">
                    <h4><span class="glyphicon glyphicon-info-sign"></span> No Enrolled Courses</h4>
                    <p>You haven't enrolled in any courses yet. Browse our training catalog to get started!</p>
                    <asp:HyperLink runat="server" NavigateUrl="~/Training.aspx" CssClass="btn btn-primary">
                        Browse Training Catalog <span class="glyphicon glyphicon-chevron-right"></span>
                    </asp:HyperLink>
                </div>
            </asp:Panel>
        </div>
    </div>
    
    <uc:Footer ID="FooterControl" runat="server" />
</asp:Content>
