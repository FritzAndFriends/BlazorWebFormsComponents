<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Home.aspx.cs" Inherits="ContosoUniversity.Home" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <link href="CSS/Home_CSS.css" rel="stylesheet" />     
    <script src="JQuery/JQuery_Home.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
    <div id="welcomeHeader">
        <h1>Welcome To Contoso University<span id="com">.com</span></h1>
        <h1>The Darkest Place In The Web...</h1>
    </div>
    <footer>
        <%: DateTime.Now.Year %>  - Contoso University&copy;
    </footer>
</asp:Content>
