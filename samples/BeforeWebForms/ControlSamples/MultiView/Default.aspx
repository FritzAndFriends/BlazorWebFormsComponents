<%@ Page Title="MultiView Sample" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.MultiView.Default" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <h2>MultiView Sample</h2>

    <div data-audit-control="MultiView-1">
        <asp:MultiView ID="MultiView1" runat="server" ActiveViewIndex="0">
            <asp:View ID="View1" runat="server">
                <h3>Step 1: Personal Information</h3>
                <p>Name: <asp:TextBox ID="txtName" runat="server" /></p>
                <p>Email: <asp:TextBox ID="txtEmail" runat="server" /></p>
                <asp:Button ID="btnNext1" runat="server" Text="Next" CommandName="NextView" />
            </asp:View>

            <asp:View ID="View2" runat="server">
                <h3>Step 2: Preferences</h3>
                <p>Color:
                    <asp:DropDownList ID="ddlColor" runat="server">
                        <asp:ListItem Text="Red" Value="Red" />
                        <asp:ListItem Text="Blue" Value="Blue" />
                        <asp:ListItem Text="Green" Value="Green" />
                    </asp:DropDownList>
                </p>
                <asp:Button ID="btnPrev2" runat="server" Text="Previous" CommandName="PrevView" />
                <asp:Button ID="btnNext2" runat="server" Text="Next" CommandName="NextView" />
            </asp:View>

            <asp:View ID="View3" runat="server">
                <h3>Step 3: Confirmation</h3>
                <p>Thank you for completing the wizard!</p>
                <asp:Button ID="btnPrev3" runat="server" Text="Previous" CommandName="PrevView" />
                <asp:Button ID="btnFinish" runat="server" Text="Finish" OnClick="btnFinish_Click" />
            </asp:View>
        </asp:MultiView>
    </div>
</asp:Content>
