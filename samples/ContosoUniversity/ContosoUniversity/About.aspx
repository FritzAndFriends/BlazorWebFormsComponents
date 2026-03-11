<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="About.aspx.cs" Inherits="ContosoUniversity.About" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
     <link href="CSS/CSS_About.css" rel="stylesheet" />
     <script src="JQuery/JQuery_About.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="header">
        <h1> Students Body Statistics </h1>
    </div>
     <div id="grv">
        <asp:GridView CssClass="grid" 
            ID="EnrollmentsStat" 
            runat="server" SelectMethod="EnrollmentsStat_GetData" 
            AutoGenerateColumns="False" EmptyDataText="There are no records to display"
             CellPadding="4" ForeColor="#333333" GridLines="None">
            <AlternatingRowStyle BackColor="White" />
            <Columns>
                <asp:BoundField DataField="Key" HeaderText="Enrollment Date" ReadOnly="True">
                <HeaderStyle BackColor="Black" BorderColor="White" BorderStyle="Solid" Font-Bold="True" Font-Size="30px" />
                <ItemStyle BorderColor="White" BorderStyle="Solid" Font-Size="20px" HorizontalAlign="Center" />
                </asp:BoundField>
                <asp:BoundField DataField="Value" HeaderText="Students" ReadOnly="True">
                <FooterStyle BorderStyle="Solid" />
                <HeaderStyle BackColor="Black" BorderColor="White" BorderStyle="Solid" Font-Bold="True" Font-Size="30px" />
                <ItemStyle BorderColor="White" BorderStyle="Solid" Font-Size="20px" HorizontalAlign="Center" />
                </asp:BoundField>
            </Columns>
            <EditRowStyle BackColor="#2461BF" />
            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
            <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
            <RowStyle BackColor="#EFF3FB" />
            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
            <SortedAscendingCellStyle BackColor="#F5F7FB" />
            <SortedAscendingHeaderStyle BackColor="#6D95E1" />
            <SortedDescendingCellStyle BackColor="#E9EBEF" />
            <SortedDescendingHeaderStyle BackColor="#4870BE" />
        </asp:GridView>
    </div>
</asp:Content>
