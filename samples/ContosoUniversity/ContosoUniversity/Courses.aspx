<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Courses.aspx.cs" Inherits="ContosoUniversity.Courses" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="CSS/CSS_Courses.css" rel="stylesheet" />
    <script src="JQuery/JQuery_Courses.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
     <asp:ScriptManager runat="server"></asp:ScriptManager>
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <div id="dropList">
                <h1 id="hdrCourse">Select Course</h1>
                <asp:DropDownList ID="drpDepartments" runat="server"></asp:DropDownList>
                <asp:Button ID="btnSearchCourse" runat="server" Text="Search Courses" OnClick="btnSearchCourse_Click"></asp:Button>
                <br />
                <br />
                <asp:GridView ID="grvCourses" runat="server" AllowPaging="True" DataKeyNames="CourseID"
                    PageSize="1" AutoGenerateColumns="False"
                    OnPageIndexChanging="grvCourses_PageIndexChanging"
                    EmptyDataText = "Chose Department To Display Courses">
                    <Columns>
                        <asp:BoundField DataField="CourseID" HeaderText="ID" />
                        <asp:BoundField DataField="CourseName" HeaderText="Course Name" />
                        <asp:BoundField DataField="StudentsMax" HeaderText="Max Capacity" />
                    </Columns>
                </asp:GridView>
            </div>
            <div id="autoComplete">
                <h1 id="crsByName">Courses By Name</h1>
                <asp:TextBox ID="txtCourse" runat="server" placeholder="Search Course..."></asp:TextBox>
                <asp:Button ID="search" runat="server" Text="Search" OnClick="search_Click"></asp:Button>
                <br />
                <br />
                <ajaxToolkit:AutoCompleteExtender
                    runat="server"
                    ID="AutoCompleteExtender1"
                    TargetControlID="txtCourse"
                    ServiceMethod="GetList"
                    MinimumPrefixLength="1"
                    CompletionInterval="100"
                    EnableCaching="true"
                    CompletionSetCount="20"
                    ShowOnlyCurrentWordInCompletionListItem="true">
                </ajaxToolkit:AutoCompleteExtender>
                <asp:DetailsView ID="dtlCourses" runat="server" Width="125px" Height="50px" CssClass="details">
                </asp:DetailsView>
        </ContentTemplate>         
    </asp:UpdatePanel>






















</asp:Content>
