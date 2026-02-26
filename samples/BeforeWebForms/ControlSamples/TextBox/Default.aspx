<%@ Page Language="C#" AutoEventWireup="true" %>
<!DOCTYPE html>
<html>
<head><title>TextBox Sample</title></head>
<body>
    <form id="form1" runat="server">
        <h2>Single Line TextBox</h2>
        <div data-audit-control="TextBox-1">
        <asp:TextBox ID="txtSingle" Text="Default Text" CssClass="form-control" runat="server" />
        </div>
        
        <h2>Password TextBox</h2>
        <div data-audit-control="TextBox-2">
        <asp:TextBox ID="txtPassword" TextMode="Password" runat="server" />
        </div>
        
        <h2>MultiLine TextBox</h2>
        <div data-audit-control="TextBox-3">
        <asp:TextBox ID="txtMulti" TextMode="MultiLine" Rows="5" Columns="40" runat="server" />
        </div>
        
        <h2>ReadOnly TextBox</h2>
        <div data-audit-control="TextBox-4">
        <asp:TextBox ID="txtReadOnly" Text="Cannot edit" ReadOnly="true" runat="server" />
        </div>
        
        <h2>Disabled TextBox</h2>
        <div data-audit-control="TextBox-5">
        <asp:TextBox ID="txtDisabled" Text="Disabled" Enabled="false" runat="server" />
        </div>
        
        <h2>With MaxLength</h2>
        <div data-audit-control="TextBox-6">
        <asp:TextBox ID="txtMaxLen" MaxLength="10" runat="server" />
        </div>
        
        <h2>With Styles</h2>
        <div data-audit-control="TextBox-7">
        <asp:TextBox ID="txtStyled" BackColor="LightYellow" ForeColor="Navy" 
                     BorderColor="Blue" BorderWidth="2" Width="200px" runat="server" />
        </div>
    </form>
</body>
</html>
