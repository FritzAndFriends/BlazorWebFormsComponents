<%@ Page Title="Test" Language="C#" %>
<asp:GridView ID="gvProducts" runat="server">
</asp:GridView>
<asp:Repeater ID="rptItems" ItemType="MyApp.Models.Product" runat="server">
</asp:Repeater>
