<%@ Page Title="Training" Language="C#" AutoEventWireup="true" CodeBehind="Training.aspx.cs" Inherits="DepartmentPortal.TrainingPage" %>
<%@ Register Src="~/Controls/PageHeader.ascx" TagName="PageHeader" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Breadcrumb.ascx" TagName="Breadcrumb" TagPrefix="uc" %>
<%@ Register Src="~/Controls/SearchBox.ascx" TagName="SearchBox" TagPrefix="uc" %>
<%@ Register Src="~/Controls/TrainingCatalog.ascx" TagName="TrainingCatalog" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Footer.ascx" TagName="Footer" TagPrefix="uc" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <uc:PageHeader ID="PageHeaderControl" runat="server" Title="Training Catalog" Description="Enhance your skills with our training courses" />
    
    <uc:Breadcrumb ID="BreadcrumbControl" runat="server" CurrentPage="Training" />
    
    <div class="row">
        <div class="col-md-8">
            <div class="row">
                <div class="col-md-6">
                    <uc:SearchBox ID="SearchBoxControl" runat="server" OnSearch="SearchBoxControl_Search" Placeholder="Search courses..." />
                </div>
            </div>
            
            <uc:TrainingCatalog ID="TrainingCatalogControl" runat="server" OnEnrollmentRequested="TrainingCatalogControl_EnrollmentRequested" />
        </div>
        
        <div class="col-md-4">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title">Quick Poll</h3>
                </div>
                <div class="panel-body">
                    <local:PollQuestion ID="PollQuestionControl" runat="server" 
                        QuestionText="What's your preferred training format?" 
                        OnVoteSubmitted="PollQuestionControl_AnswerSubmitted" />
                </div>
            </div>
            
            <div class="panel panel-info">
                <div class="panel-heading">
                    <h3 class="panel-title">My Enrollments</h3>
                </div>
                <div class="panel-body">
                    <p>You are enrolled in <strong><asp:Label ID="EnrollmentCountLabel" runat="server" Text="0" /></strong> courses.</p>
                    <asp:HyperLink runat="server" NavigateUrl="~/MyTraining.aspx" CssClass="btn btn-info btn-sm">
                        View My Courses <span class="glyphicon glyphicon-chevron-right"></span>
                    </asp:HyperLink>
                </div>
            </div>
        </div>
    </div>
    
    <uc:Footer ID="FooterControl" runat="server" />
</asp:Content>
