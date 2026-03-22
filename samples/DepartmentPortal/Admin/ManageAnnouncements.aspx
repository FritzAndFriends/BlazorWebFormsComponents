<%@ Page Title="Manage Announcements" Language="C#" AutoEventWireup="true" CodeFile="ManageAnnouncements.aspx.cs" Inherits="DepartmentPortal.Admin.ManageAnnouncementsPage" %>
<%@ Register Src="~/Controls/PageHeader.ascx" TagName="PageHeader" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Breadcrumb.ascx" TagName="Breadcrumb" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Footer.ascx" TagName="Footer" TagPrefix="uc" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <uc:PageHeader ID="PageHeaderControl" runat="server" Title="Manage Announcements" Description="Create, edit, and manage announcements" />
    
    <uc:Breadcrumb ID="BreadcrumbControl" runat="server" CurrentPage="Manage Announcements" />
    
    <div class="row">
        <div class="col-md-12">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title">Announcement List</h3>
                </div>
                <div class="panel-body">
                    <asp:Button ID="AddNewButton" runat="server" Text="Add New Announcement" CssClass="btn btn-primary" OnClick="AddNewButton_Click" />
                    
                    <asp:GridView ID="AnnouncementsGridView" runat="server" 
                        AutoGenerateColumns="false" 
                        CssClass="table table-striped table-hover" 
                        OnRowEditing="AnnouncementsGridView_RowEditing"
                        OnRowDeleting="AnnouncementsGridView_RowDeleting"
                        OnRowCommand="AnnouncementsGridView_RowCommand"
                        DataKeyNames="Id">
                        <Columns>
                            <asp:BoundField DataField="Id" HeaderText="ID" ReadOnly="true" />
                            <asp:BoundField DataField="Title" HeaderText="Title" />
                            <asp:BoundField DataField="Author" HeaderText="Author" />
                            <asp:BoundField DataField="PublishDate" HeaderText="Publish Date" DataFormatString="{0:MM/dd/yyyy}" />
                            <asp:CheckBoxField DataField="IsActive" HeaderText="Active" />
                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" CommandName="Edit" CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-sm btn-default">
                                        <span class="glyphicon glyphicon-edit"></span> Edit
                                    </asp:LinkButton>
                                    <asp:LinkButton runat="server" CommandName="Delete" CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-sm btn-danger" OnClientClick="return confirm('Are you sure you want to delete this announcement?');">
                                        <span class="glyphicon glyphicon-trash"></span> Delete
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
            
            <asp:Panel ID="EditPanel" runat="server" Visible="false" CssClass="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title">
                        <asp:Label ID="EditPanelTitle" runat="server" Text="Add Announcement" />
                    </h3>
                </div>
                <div class="panel-body">
                    <asp:HiddenField ID="EditAnnouncementId" runat="server" Value="0" />
                    
                    <div class="form-group">
                        <label>Title</label>
                        <asp:TextBox ID="TitleTextBox" runat="server" CssClass="form-control" MaxLength="200" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="TitleTextBox" ErrorMessage="Title is required" CssClass="text-danger" Display="Dynamic" />
                    </div>
                    
                    <div class="form-group">
                        <label>Body</label>
                        <asp:TextBox ID="BodyTextBox" runat="server" TextMode="MultiLine" Rows="10" CssClass="form-control" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="BodyTextBox" ErrorMessage="Body is required" CssClass="text-danger" Display="Dynamic" />
                    </div>
                    
                    <div class="form-group">
                        <label>Author</label>
                        <asp:TextBox ID="AuthorTextBox" runat="server" CssClass="form-control" MaxLength="100" />
                    </div>
                    
                    <div class="form-group">
                        <label>Publish Date</label>
                        <asp:TextBox ID="PublishDateTextBox" runat="server" TextMode="Date" CssClass="form-control" />
                    </div>
                    
                    <div class="checkbox">
                        <label>
                            <asp:CheckBox ID="IsActiveCheckBox" runat="server" /> Active
                        </label>
                    </div>
                    
                    <asp:Button ID="SaveButton" runat="server" Text="Save" CssClass="btn btn-success" OnClick="SaveButton_Click" />
                    <asp:Button ID="CancelButton" runat="server" Text="Cancel" CssClass="btn btn-default" OnClick="CancelButton_Click" CausesValidation="false" />
                </div>
            </asp:Panel>
        </div>
    </div>
    
    <uc:Footer ID="FooterControl" runat="server" />
</asp:Content>
