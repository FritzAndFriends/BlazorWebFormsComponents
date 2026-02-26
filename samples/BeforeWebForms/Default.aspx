<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <h1>Before Web Forms</h1>

    <p>
        This is a project that contains individual pages for the BlazorWebFormsComponents developers to use
        to test the existing controls and see how they behave.  We will use this project to determine how best to build
        our matching Blazor components.
    </p>

    <div class="row">
        <div class="col-md-12">
            <a href="https://github.com/FritzAndFriends/BlazorWebFormsComponents">BlazorWebFormsComponents GitHub</a>
        </div>
    </div>

    <div class="row">
        <div class="col-md-12">
            <a href="https://fritzandfriends.github.io/BlazorWebFormsComponents/">Online BlazorWebFormsComponents Documentation</a>
        </div>
    </div>

    <div class="row">

        <div class="col-md-3">
            <h3>Data Controls</h3>
            <ul>
                <li>Chart(?)</li>
                <li><a href="ControlSamples/DataList/Default.aspx">DataList</a></li>
                <li>DataPager</li>
                <li>DetailsView</li>
                <li><a href="ControlSamples/FormView">FormView</a></li>
                <li><a href="ControlSamples/GridView/Default.aspx">GridView</a></li>
                <li><a href="ControlSamples/ListView">ListView</a></li>
                <li><a href="ControlSamples/Repeater/Default.aspx">Repeater</a></li>
            </ul>

            <h3>Editor Controls</h3>
            <ul>
              <li><a href="ControlSamples/Button/Default.aspx">Button</a></li>
              <li><a href="ControlSamples/Hyperlink/Default.aspx">Hyperlink</a></li>
            </ul>

        <div class="col-md-3">
            <h3>Validation Controls</h3>
            <ul>
                <li>CompareValidator</li>
                <li>CustomValidator</li>
                <li>RangeValidator</li>
                <li>RegularExpressionValidator(?)</li>
                <li>RequiredFieldValidator</li>
                <li>ValidationSummary</li>
            </ul>
        </div>

        <div class="col-md-3">
            <h3>Navigation Controls</h3>
            <ul>
                <li><a href="ControlSamples/Menu">Menu</a></li>
                <li>SiteMapPath</li>
                <li><a href="ControlSamples/TreeView">TreeView</a></li>
            </ul>
        </div>

        <div class="col-md=3">
            <h3>Login Controls</h3>
            <ul>
            </ul>
        </div>
    </div>
</asp:Content>
