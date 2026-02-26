<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.Hyperlink._default" MasterPageFile="~/Site.Master" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">

<h2>Hyperlink Demos</h2>

	<p>
		This hyperlink has Style!

		<div data-audit-control="HyperLink-1">
		<asp:HyperLink runat="server" ID="styleLink" BackColor="Blue" ForeColor="White" Text="Blue Button" NavigateUrl="https://bing.com" />  
		</div>

	</p>

	<p>
		This hyperlink has Tooltips!

		<div data-audit-control="HyperLink-2">
		<asp:HyperLink runat="server" ID="HyperLink1" Text="Blue Button" ToolTip="Navigate to Bing!" NavigateUrl="https://bing.com" />  
		</div>

	</p>

	<p>
		This hyperlink is not visible

		<div data-audit-control="HyperLink-3">
		<asp:HyperLink runat="server" ID="HyperLink2" Text="Blue Button" Visible="false" NavigateUrl="https://bing.com" />  
		</div>

	</p>

	<p>

		This hyperlink is DataBinding!

		<div data-audit-control="HyperLink-4">
		<asp:HyperLink runat="server" ID="HyperLink3" Text="Blue Button" OnDataBinding="HyperLink3_DataBinding" NavigateUrl="https://bing.com" />  
		</div>

	</p>

</asp:Content>
