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


</asp:Content>
