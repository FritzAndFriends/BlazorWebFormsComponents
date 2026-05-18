<%@ Page Title="Manage Training" Language="C#" AutoEventWireup="true" CodeBehind="ManageTraining.aspx.cs" Inherits="DepartmentPortal.Admin.ManageTrainingPage" %>
<%@ Register Src="~/Controls/PageHeader.ascx" TagName="PageHeader" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Breadcrumb.ascx" TagName="Breadcrumb" TagPrefix="uc" %>
<%@ Register Src="~/Controls/Footer.ascx" TagName="Footer" TagPrefix="uc" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <uc:PageHeader ID="PageHeaderControl" runat="server" Title="Manage Training" Description="Manage training courses and categories" />
    
    <uc:Breadcrumb ID="BreadcrumbControl" runat="server" CurrentPage="Manage Training" />
    
    <div class="row">
        <div class="col-md-12">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title">Training Courses</h3>
                </div>
                <div class="panel-body">
                    <asp:Button ID="AddNewCourseButton" runat="server" Text="Add New Course" CssClass="btn btn-primary" OnClick="AddNewCourseButton_Click" />
                    
                    <asp:GridView ID="CoursesGridView" runat="server" 
                        AutoGenerateColumns="false" 
                        CssClass="table table-striped table-hover" 
                        OnRowCommand="CoursesGridView_RowCommand"
                        DataKeyNames="Id">
                        <Columns>
                            <asp:BoundField DataField="Id" HeaderText="ID" ReadOnly="true" />
                            <asp:BoundField DataField="CourseName" HeaderText="Course Name" />
                            <asp:BoundField DataField="Category" HeaderText="Category" />
                            <asp:BoundField DataField="DurationHours" HeaderText="Duration (hrs)" />
                            <asp:BoundField DataField="InstructorName" HeaderText="Instructor" />
                            <asp:CheckBoxField DataField="IsAvailable" HeaderText="Available" />
                            <asp:TemplateField HeaderText="Actions">
                                <ItemTemplate>
                                    <asp:LinkButton runat="server" CommandName="EditCourse" CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-sm btn-default">
                                        <span class="glyphicon glyphicon-edit"></span> Edit
                                    </asp:LinkButton>
                                    <asp:LinkButton runat="server" CommandName="DeleteCourse" CommandArgument='<%# Eval("Id") %>' CssClass="btn btn-sm btn-danger" OnClientClick="return confirm('Are you sure you want to delete this course?');">
                                        <span class="glyphicon glyphicon-trash"></span> Delete
                                    </asp:LinkButton>
                                </ItemTemplate>
                            </asp:TemplateField>
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
            
            <asp:Panel ID="EditCoursePanel" runat="server" Visible="false" CssClass="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title">
                        <asp:Label ID="EditCoursePanelTitle" runat="server" Text="Add Course" />
                    </h3>
                </div>
                <div class="panel-body">
                    <asp:HiddenField ID="EditCourseId" runat="server" Value="0" />
                    
                    <div class="form-group">
                        <label>Course Name</label>
                        <asp:TextBox ID="CourseNameTextBox" runat="server" CssClass="form-control" MaxLength="200" />
                        <asp:RequiredFieldValidator runat="server" ControlToValidate="CourseNameTextBox" ErrorMessage="Course name is required" CssClass="text-danger" Display="Dynamic" />
                    </div>
                    
                    <div class="form-group">
                        <label>Description</label>
                        <asp:TextBox ID="DescriptionTextBox" runat="server" TextMode="MultiLine" Rows="5" CssClass="form-control" />
                    </div>
                    
                    <div class="row">
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>Category</label>
                                <asp:TextBox ID="CategoryTextBox" runat="server" CssClass="form-control" />
                            </div>
                        </div>
                        <div class="col-md-6">
                            <div class="form-group">
                                <label>Duration (hours)</label>
                                <asp:TextBox ID="DurationTextBox" runat="server" TextMode="Number" CssClass="form-control" />
                            </div>
                        </div>
                    </div>
                    
                    <div class="form-group">
                        <label>Instructor Name</label>
                        <asp:TextBox ID="InstructorTextBox" runat="server" CssClass="form-control" />
                    </div>
                    
                    <div class="checkbox">
                        <label>
                            <asp:CheckBox ID="IsAvailableCheckBox" runat="server" /> Available for enrollment
                        </label>
                    </div>
                    
                    <asp:Button ID="SaveCourseButton" runat="server" Text="Save" CssClass="btn btn-success" OnClick="SaveCourseButton_Click" />
                    <asp:Button ID="CancelCourseButton" runat="server" Text="Cancel" CssClass="btn btn-default" OnClick="CancelCourseButton_Click" CausesValidation="false" />
                </div>
            </asp:Panel>
        </div>
    </div>
    
    <uc:Footer ID="FooterControl" runat="server" />
</asp:Content>
