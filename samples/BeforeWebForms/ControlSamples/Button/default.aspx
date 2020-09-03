<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.Button._default" %>

<asp:Content runat="server" ContentPlaceHolderID="MainContent">

	<h2>Button Demos</h2>

	<p>
	This button has STYLE!

		<asp:Button runat="server" ID="styleButton" BackColor="Blue" ForeColor="White" Text="Blue Button" />

	</p>

	<p>

		This button does NOT cause validation:
		<asp:Button runat="server" ID="noValidationButton" CausesValidation="false" Text="No Validation for you!" />

	</p>


	<p>

		This button is not Enabled:
		<asp:Button runat="server" ID="notEnabled" Enabled="false" Text="I am not enabled for clicking" />

		<br />
		This button is Enabled:
		<asp:Button runat="server" ID="Button1" Enabled="true" Text="I am enabled for clicking" />

	</p>

	<p>
		This button posts to Bing
		<asp:Button runat="server" ID="bingPostButton" PostBackUrl="www.bing.com" Text="Post to Bing" />
	</p>


</asp:Content>
