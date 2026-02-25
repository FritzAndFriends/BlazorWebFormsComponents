<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/Site.Master" CodeBehind="Default.aspx.cs" Inherits="BeforeWebForms.ControlSamples.Calendar._default" %>

<asp:Content ContentPlaceHolderID="MainContent" runat="server">

    <h2>Calendar Control Samples</h2>

    <h3>Basic Calendar</h3>
    <asp:Calendar ID="Calendar1" runat="server" />

    <h3>Selection Modes</h3>

    <h4>Day Selection (Default)</h4>
    <asp:Calendar ID="CalendarDay" runat="server" 
        SelectionMode="Day" />

    <h4>DayWeek — Select Entire Weeks</h4>
    <asp:Calendar ID="CalendarWeek" runat="server" 
        SelectionMode="DayWeek" />

    <h4>DayWeekMonth — Select Weeks or Month</h4>
    <asp:Calendar ID="CalendarMonth" runat="server" 
        SelectionMode="DayWeekMonth" />

    <h3>Styled Calendar</h3>
    <asp:Calendar ID="CalendarStyled" runat="server"
        BackColor="White" 
        BorderColor="#999999" 
        CellPadding="4"
        DayNameFormat="Shortest" 
        Font-Names="Verdana" 
        Font-Size="8pt"
        ForeColor="Black" 
        ShowGridLines="true"
        Width="220px">
        <TitleStyle BackColor="#CCCCFF" Font-Bold="True" ForeColor="#333399" Font-Size="13pt" />
        <DayHeaderStyle BackColor="#CCCCFF" Font-Bold="True" Font-Size="7pt" />
        <SelectedDayStyle BackColor="#009999" Font-Bold="True" ForeColor="White" />
        <TodayDayStyle BackColor="#CCCCFF" ForeColor="Black" />
        <OtherMonthDayStyle ForeColor="#999999" />
        <WeekendDayStyle BackColor="#FFFFCC" />
        <NextPrevStyle Font-Size="8pt" ForeColor="#333399" VerticalAlign="Bottom" />
        <SelectorStyle BackColor="#CCCCFF" />
    </asp:Calendar>

    <h3>Custom Navigation Text</h3>
    <asp:Calendar ID="CalendarNav" runat="server"
        NextMonthText="Next &raquo;"
        PrevMonthText="&laquo; Prev" />

    <h3>Events</h3>
    <asp:Calendar ID="CalendarEvents" runat="server"
        OnSelectionChanged="CalendarEvents_SelectionChanged"
        OnVisibleMonthChanged="CalendarEvents_VisibleMonthChanged" />
    <asp:Label ID="lblSelected" runat="server" />
    <asp:Label ID="lblMonthChanged" runat="server" />

</asp:Content>
