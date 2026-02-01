<%@ Page Title="Chart - Pie Chart" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PieChart.aspx.cs" Inherits="BeforeWebForms.ControlSamples.Chart.PieChart" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Chart Control - Pie Chart</h2>

    <div>
        <a href="Default.aspx">Column Chart</a> | <a href="PieChart.aspx">Pie Chart</a> | <a href="LineChart.aspx">Line Chart</a> | <a href="DataBinding.aspx">Data Binding</a>
    </div>

    <p>This is an example of a Pie Chart showing market share distribution.</p>

    <asp:Chart ID="Chart1" runat="server" Width="600px" Height="400px" 
        BackColor="WhiteSmoke" BorderColor="#999999" BorderWidth="1" 
        BorderDashStyle="Solid">
        <Series>
            <asp:Series Name="MarketShare" ChartType="Pie" 
                BorderColor="Black" BorderWidth="1">
                <SmartLabelStyle Enabled="True" />
            </asp:Series>
        </Series>
        <ChartAreas>
            <asp:ChartArea Name="ChartArea1" BackColor="White">
            </asp:ChartArea>
        </ChartAreas>
        <Titles>
            <asp:Title Text="Product Market Share" Font="Arial, 14pt, style=Bold" />
        </Titles>
        <Legends>
            <asp:Legend Name="Legend1" Docking="Right" Alignment="Center" />
        </Legends>
    </asp:Chart>

</asp:Content>
