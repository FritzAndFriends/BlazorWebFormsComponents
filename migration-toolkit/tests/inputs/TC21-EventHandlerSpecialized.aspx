<%@ Page Title="Test" Language="C#" CodeBehind="TC21-EventHandlerSpecialized.aspx.cs" Inherits="MyApp.TC21_EventHandlerSpecialized" %>
<asp:GridView ID="gvItems" runat="server" OnRowCommand="Grid_RowCommand" OnPageIndexChanging="Grid_PageIndexChanging">
</asp:GridView>
