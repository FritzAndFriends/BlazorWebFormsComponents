<%@ Page Title="Ajax Test" Language="C#" %>
<%@ Register TagPrefix="ajaxToolkit" Namespace="AjaxControlToolkit" Assembly="AjaxControlToolkit" %>
<ajaxToolkit:ToolkitScriptManager runat="server" />
<ajaxToolkit:TabContainer ID="TabContainer1" runat="server">
    <ajaxToolkit:TabPanel HeaderText="Tab 1">
        <ContentTemplate>
            <p>Tab content</p>
        </ContentTemplate>
    </ajaxToolkit:TabPanel>
</ajaxToolkit:TabContainer>
<ajaxToolkit:RatingExtender ID="Rating1" TargetControlID="TextBox1" runat="server" />
