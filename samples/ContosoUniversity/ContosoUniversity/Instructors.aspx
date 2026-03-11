<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Instructors.aspx.cs" Inherits="ContosoUniversity.Instructors" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="CSS/Instructors_CSS.css" rel="stylesheet" />
    <script src="JQuery/JQuery_Instructors.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
   <asp:ScriptManager runat="server"></asp:ScriptManager>
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <div id="grv">
                <h1 id="hdr">Our Instructors</h1>
                <asp:GridView ID="grvInstructors" runat="server" AutoGenerateColumns="False"
                    CssClass="grv" BorderWidth="2px" Font-Size="30px"                 
                    AllowSorting="true" OnSorting="grvInstructors_Sorting" 
                    EmptyDataText = "There are no records to display" >
                    <Columns>
                        <asp:BoundField DataField="InstructorID" HeaderText="Instructor ID" SortExpression="InstructorID" />
                        <asp:BoundField DataField="FirstName" HeaderText="First Name"  SortExpression="FirstName"/>
                        <asp:BoundField DataField="LastName" HeaderText="Last Name"  SortExpression="LastName"/>
                        <asp:BoundField DataField="Email" HeaderText="Email"  />
                    </Columns>
                </asp:GridView>
            </div>
        </ContentTemplate>
        </asp:UpdatePanel>
</asp:Content>
