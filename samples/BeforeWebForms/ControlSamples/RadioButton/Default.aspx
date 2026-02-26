<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.RadioButton.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>RadioButton Control Samples</h2>
    
    <h3>Group 1 - Option A (Checked)</h3>
    <div data-audit-control="RadioButton-1">
    <asp:RadioButton ID="rbOptionA" Text="Option A" GroupName="group1" Checked="true" runat="server" />
    </div>
    
    <h3>Group 1 - Option B</h3>
    <div data-audit-control="RadioButton-2">
    <asp:RadioButton ID="rbOptionB" Text="Option B" GroupName="group1" runat="server" />
    </div>
    
    <h3>Standalone (Group 2)</h3>
    <div data-audit-control="RadioButton-3">
    <asp:RadioButton ID="rbStandalone" Text="Standalone" GroupName="group2" runat="server" />
    </div>
</asp:Content>
