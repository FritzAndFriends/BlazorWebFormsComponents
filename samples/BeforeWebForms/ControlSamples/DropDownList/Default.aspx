<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.DropDownList.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>DropDownList Control Samples</h2>
    
    <h3>Basic DropDownList with Static Items</h3>
    <asp:DropDownList ID="ddlStatic" runat="server">
        <asp:ListItem Text="Select..." Value="" />
        <asp:ListItem Text="Option One" Value="1" />
        <asp:ListItem Text="Option Two" Value="2" />
        <asp:ListItem Text="Option Three" Value="3" />
    </asp:DropDownList>
    
    <h3>With Pre-selected Value</h3>
    <asp:DropDownList ID="ddlSelected" runat="server">
        <asp:ListItem Text="Apple" Value="apple" />
        <asp:ListItem Text="Banana" Value="banana" Selected="True" />
        <asp:ListItem Text="Cherry" Value="cherry" />
    </asp:DropDownList>
    
    <h3>Data-bound DropDownList</h3>
    <asp:DropDownList ID="ddlDataBound" DataTextField="Name" DataValueField="Id" runat="server" />
    
    <h3>Disabled DropDownList</h3>
    <asp:DropDownList ID="ddlDisabled" Enabled="false" runat="server">
        <asp:ListItem Text="Cannot change" Value="1" />
    </asp:DropDownList>
    
    <h3>With CSS Class</h3>
    <asp:DropDownList ID="ddlStyled" CssClass="form-select" runat="server">
        <asp:ListItem Text="Styled" Value="1" />
    </asp:DropDownList>
    
    <h3>With Inline Styles</h3>
    <asp:DropDownList ID="ddlColors" BackColor="LightYellow" ForeColor="Navy" 
                      Width="200px" runat="server">
        <asp:ListItem Text="Colored dropdown" Value="1" />
    </asp:DropDownList>
</asp:Content>
