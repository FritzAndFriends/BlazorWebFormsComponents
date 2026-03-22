<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TrainingCatalog.ascx.cs" Inherits="DepartmentPortal.Controls.TrainingCatalog" %>

<div class="training-catalog">
    <asp:Repeater ID="rptCourses" runat="server" OnItemCommand="rptCourses_ItemCommand">
        <ItemTemplate>
            <div class="course-card">
                <h4><%# Eval("CourseName") %></h4>
                <p class="course-description"><%# Eval("Description") %></p>
                <div class="course-meta">
                    <span class="instructor">Instructor: <%# Eval("Instructor") %></span>
                    <span class="duration"><%# Eval("DurationHours") %> hours</span>
                    <span class="category"><%# Eval("Category") %></span>
                </div>
                <asp:Button ID="btnEnroll" runat="server" Text="Enroll" CssClass="btn btn-enroll"
                    CommandName="Enroll" CommandArgument='<%# Eval("Id") %>' />
            </div>
        </ItemTemplate>
    </asp:Repeater>
</div>
