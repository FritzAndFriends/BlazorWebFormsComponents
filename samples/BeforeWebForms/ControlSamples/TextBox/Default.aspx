<%@ Page Language="C#" AutoEventWireup="true" %>
<!DOCTYPE html>
<html>
<head><title>TextBox Sample</title></head>
<body>
    <form id="form1" runat="server">
        <h2>Single Line TextBox</h2>
        <asp:TextBox ID="txtSingle" Text="Default Text" CssClass="form-control" runat="server" />
        
        <h2>Password TextBox</h2>
        <asp:TextBox ID="txtPassword" TextMode="Password" runat="server" />
        
        <h2>MultiLine TextBox</h2>
        <asp:TextBox ID="txtMulti" TextMode="MultiLine" Rows="5" Columns="40" runat="server" />
        
        <h2>ReadOnly TextBox</h2>
        <asp:TextBox ID="txtReadOnly" Text="Cannot edit" ReadOnly="true" runat="server" />
        
        <h2>Disabled TextBox</h2>
        <asp:TextBox ID="txtDisabled" Text="Disabled" Enabled="false" runat="server" />
        
        <h2>With MaxLength</h2>
        <asp:TextBox ID="txtMaxLen" MaxLength="10" runat="server" />
        
        <h2>With Styles</h2>
        <asp:TextBox ID="txtStyled" BackColor="LightYellow" ForeColor="Navy" 
                     BorderColor="Blue" BorderWidth="2" Width="200px" runat="server" />
    </form>
</body>
</html>
