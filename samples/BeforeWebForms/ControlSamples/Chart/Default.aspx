<%@ Page Title="Chart - Column Chart" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.Chart.Default" %>
<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2>Chart Control - Column Chart</h2>

    <div>
        <a href="Default.aspx">Column Chart</a> | <a href="PieChart.aspx">Pie Chart</a> | <a href="LineChart.aspx">Line Chart</a> | <a href="DataBinding.aspx">Data Binding</a>
    </div>

    <p>This is a basic example of a Column Chart displaying monthly sales data.</p>

    <div data-audit-control="Chart">
        <asp:Chart ID="Chart1" runat="server" Width="600px" Height="400px" 
            BackColor="WhiteSmoke" BorderColor="#999999" BorderWidth="1" 
            BorderDashStyle="Solid">
            <Series>
                <asp:Series Name="Sales" ChartType="Column" 
                    Color="SteelBlue" BorderColor="Black" BorderWidth="1">
                </asp:Series>
            </Series>
            <ChartAreas>
                <asp:ChartArea Name="ChartArea1" 
                    BackColor="White" 
                    BorderColor="Black" 
                    BorderWidth="1" 
                    BorderDashStyle="Solid">
                    <AxisX Title="Month">
                        <MajorGrid LineColor="LightGray" />
                    </AxisX>
                    <AxisY Title="Sales ($)">
                        <MajorGrid LineColor="LightGray" />
                    </AxisY>
                </asp:ChartArea>
            </ChartAreas>
            <Titles>
                <asp:Title Text="Monthly Sales Report" Font="Arial, 14pt, style=Bold" />
            </Titles>
        </asp:Chart>
    </div>

</asp:Content>
