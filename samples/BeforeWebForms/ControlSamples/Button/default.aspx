<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeFile="default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.Button._default" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">

	<h2>Button Demos</h2>

	<p>
	This button has STYLE!

		<div data-audit-control="Button-1">
		<asp:Button runat="server" ID="styleButton" BackColor="Blue" ForeColor="White" Text="Blue Button" />
		</div>

	</p>

	<p>

		This button does NOT cause validation:
		<div data-audit-control="Button-2">
		<asp:Button runat="server" ID="noValidationButton" CausesValidation="false" Text="No Validation for you!" />
		</div>

	</p>


	<p>

		This button is not Enabled:
		<div data-audit-control="Button-3">
		<asp:Button runat="server" ID="notEnabled" Enabled="false" Text="I am not enabled for clicking" />
		</div>

		<br />
		This button is Enabled:
		<div data-audit-control="Button-4">
		<asp:Button runat="server" ID="Button1" Enabled="true" Text="I am enabled for clicking" />
		</div>

	</p>

	<p>
		This button posts to Bing
		<div data-audit-control="Button-5">
		<asp:Button runat="server" ID="bingPostButton" PostBackUrl="www.bing.com" Text="Post to Bing" />
		</div>
	</p>


</asp:Content>
