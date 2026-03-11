<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Students.aspx.cs" Inherits="ContosoUniversity.Students" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajaxToolkit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="CSS/Students_CSS.css" rel="stylesheet" />
    <script src="JQuery/JQuery_Students.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <asp:ScriptManager ID="ScriptManager2" runat="server"></asp:ScriptManager>       
    <asp:UpdatePanel runat="server">
        <ContentTemplate>
            <div id="grvStudentsData">
                <asp:GridView ID="grv"
                    CssClass="grv" runat="server"
                    AutoGenerateColumns="False"
                    EmptyDataText="There are no records to display"
                    SelectMethod="grv_GetData" UpdateMethod="grv_UpdateItem"
                    DeleteMethod="grv_DeleteItem" DataKeyNames="ID" OnRowUpdating="grv_RowUpdating"
                    BackColor="White" BorderColor="#3366CC" BorderStyle="None" BorderWidth="1px"
                    CellPadding="4" Height="336px" Width="416px">
                    <Columns>
                        <asp:CommandField HeaderText="Delete Student" ShowDeleteButton="True" ButtonType="Button" />
                        <asp:BoundField DataField="ID" HeaderText="Student ID" ReadOnly="True" />
                        <asp:BoundField DataField="FullName" HeaderText="Student Name" />
                        <asp:BoundField DataField="Email" HeaderText="Email" />
                        <asp:BoundField DataField="Date" HeaderText="Enrollment Date" ReadOnly="True" />
                        <asp:BoundField DataField="Count" HeaderText="Number of Courses" ReadOnly="True" />
                        <asp:CommandField ButtonType="Button" HeaderText="Update Student Peronal Data" ShowEditButton="True" />
                    </Columns>
                    <FooterStyle BackColor="#99CCCC" ForeColor="#003399" />
                    <HeaderStyle BackColor="#003399" Font-Bold="True" ForeColor="#CCCCFF" />
                    <PagerStyle BackColor="#99CCCC" ForeColor="#003399" HorizontalAlign="Left" />
                    <RowStyle BackColor="White" ForeColor="#003399" />
                    <SelectedRowStyle BackColor="#009999" Font-Bold="True" ForeColor="#CCFF99" />
                    <SortedAscendingCellStyle BackColor="#EDF6F6" />
                    <SortedAscendingHeaderStyle BackColor="#0D4AC4" />
                    <SortedDescendingCellStyle BackColor="#D6DFDF" />
                    <SortedDescendingHeaderStyle BackColor="#002876" />
                </asp:GridView>
            </div>
            <div id="addStud">
                <h1 id="addStudent">Add Student</h1>
                <asp:Table ID="tabAddStud" runat="server"
                    CssClass="tabAddStud" BorderColor="#0033CC" Font-Bold="True" Font-Italic="False"
                    Font-Underline="False" ForeColor="White"
                    Font-Overline="False">
                    <asp:TableRow runat="server">
                        <asp:TableCell runat="server">First Name</asp:TableCell>
                        <asp:TableCell runat="server">
                            <asp:TextBox ID="txtFirstName" placeholder="Type Your First Name..." runat="server" />
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow runat="server">
                        <asp:TableCell runat="server">Last Name</asp:TableCell><asp:TableCell runat="server">
                            <asp:TextBox ID="txtLastName" placeholder="Type Your Last Name..." runat="server" />
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow runat="server">
                        <asp:TableCell runat="server">Birth Date</asp:TableCell><asp:TableCell runat="server">
                            <asp:TextBox ID="txtBirthDate" placeholder="dd.mm.year" runat="server" />
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow runat="server">
                        <asp:TableCell runat="server">Email</asp:TableCell><asp:TableCell runat="server">
                            <asp:TextBox ID="txtEmail" placeholder="Email is Optional..." runat="server" />
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow runat="server">
                        <asp:TableCell runat="server">Course Name</asp:TableCell><asp:TableCell runat="server">
                            <asp:DropDownList ID="dropListCourses" runat="server">
                            </asp:DropDownList>
                        </asp:TableCell>
                    </asp:TableRow>
                    <asp:TableRow runat="server">
                        <asp:TableCell runat="server">               
                        </asp:TableCell><asp:TableCell runat="server">
                            <asp:Button CssClass="insert" ID="btnInsert" Text="New Enrollment" runat="server" OnClick="btnInsert_Click" />
                            &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                     <asp:Button ID="btnClear" Text="Clear" runat="server" OnClick="btnClear_Click" />
                        </asp:TableCell>
                    </asp:TableRow>
                </asp:Table>
            </div>
            <div id="ajax">
                <h1 id="searchStud">Search Student</h1>
                <asp:TextBox CssClass="txtSearch" ID="txtSearch" runat="server" placeholder="Type Student Name..." />
                <ajaxToolkit:AutoCompleteExtender
                    runat="server"
                    ID="AutoCompleteExtender1"
                    TargetControlID="txtSearch"
                    ServiceMethod="GetCompletionList"
                    MinimumPrefixLength="1"
                    CompletionInterval="100"
                    EnableCaching="true"
                    CompletionSetCount="20"
                    ShowOnlyCurrentWordInCompletionListItem="true">
                </ajaxToolkit:AutoCompleteExtender>
                <asp:Button ID="btnSearch" Text="Show Student Info" runat="server" OnClick="btnSearch_Click" CssClass="btnInfo" />
                <asp:DetailsView
                    runat="server"
                    ID="studentData"
                    Width="125px"
                    Height="50px"
                    AutoGenerateRows="False"
                    CssClass="detailsView" BorderColor="Black" ForeColor="Black">
                    <Fields>
                        <asp:BoundField
                            DataField="FirstName"
                            HeaderText="First Name" />
                        <asp:BoundField DataField="LastName" HeaderText="Last Name" />
                        <asp:BoundField DataField="Email" HeaderText="Email" />
                        <asp:BoundField DataField="BirthDate" HeaderText="Birth Date" />
                        <asp:BoundField DataField="StudentID" HeaderText="id" />
                    </Fields>
                    <RowStyle BackColor="White" />
                </asp:DetailsView>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>         
     
</asp:Content>