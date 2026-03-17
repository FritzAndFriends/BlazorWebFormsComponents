<%@ Page Title="Test" Language="C#" %>
<asp:GridView ID="gvProducts" DataSourceID="SqlDS1" runat="server">
</asp:GridView>
<asp:SqlDataSource ID="SqlDS1" runat="server"
    ConnectionString="<%$ ConnectionStrings:MyDB %>"
    SelectCommand="SELECT * FROM Products" />
