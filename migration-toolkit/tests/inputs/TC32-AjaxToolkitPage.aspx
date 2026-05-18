<%@ Page Title="Customer Management" Language="C#" MasterPageFile="~/Site.Master" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">
    <ajaxToolkit:ToolkitScriptManager ID="ScriptManager1" runat="server" />
    <h1>Customer Management</h1>
    <ajaxToolkit:TabContainer ID="TabContainer1" ActiveTabIndex="0" CssClass="tabs" runat="server">
        <ajaxToolkit:TabPanel ID="TabSearch" HeaderText="Search" runat="server">
            <ContentTemplate>
                <div class="form-group">
                    <asp:TextBox ID="txtCustomerSearch" CssClass="form-control" runat="server" />
                    <ajaxToolkit:AutoCompleteExtender ID="aceSearch" TargetControlID="txtCustomerSearch"
                        ServiceMethod="GetCustomerSuggestions" MinimumPrefixLength="2" runat="server" />
                </div>
                <asp:Button ID="btnSearch" Text="Search" OnClick="Search_Click" runat="server" />
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
        <ajaxToolkit:TabPanel ID="TabDetails" HeaderText="Details" runat="server">
            <ContentTemplate>
                <asp:Label ID="lblCustomerName" runat="server" />
                <asp:TextBox ID="txtEmail" runat="server" />
            </ContentTemplate>
        </ajaxToolkit:TabPanel>
    </ajaxToolkit:TabContainer>
    <ajaxToolkit:Accordion ID="Accordion1" SelectedIndex="0" FadeTransitions="True" runat="server">
        <Panes>
            <ajaxToolkit:AccordionPane ID="paneNotes" runat="server">
                <Header>Customer Notes</Header>
                <Content>
                    <asp:TextBox ID="txtNotes" TextMode="MultiLine" Rows="5" CssClass="form-control" runat="server" />
                </Content>
            </ajaxToolkit:AccordionPane>
        </Panes>
    </ajaxToolkit:Accordion>
    <asp:Button ID="btnDelete" Text="Delete Customer" OnClick="Delete_Click" runat="server" />
    <ajaxToolkit:ConfirmButtonExtender ID="cbeDelete" TargetControlID="btnDelete"
        ConfirmText="Are you sure you want to delete this customer?" runat="server" />
    <asp:Panel ID="pnlModal" CssClass="modal-content" runat="server" Visible="False">
        <h3>Confirm Action</h3>
        <asp:Button ID="btnConfirm" Text="Confirm" OnClick="Confirm_Click" runat="server" />
        <asp:Button ID="btnCancel" Text="Cancel" runat="server" />
    </asp:Panel>
    <ajaxToolkit:ModalPopupExtender ID="mpeConfirm" TargetControlID="btnDelete"
        PopupControlID="pnlModal" CancelControlID="btnCancel" runat="server" />
    <%-- Rating control has no BWFC equivalent --%>
    <ajaxToolkit:Rating ID="CustomerRating" MaxRating="5" CurrentRating="3" runat="server" />
</asp:Content>
