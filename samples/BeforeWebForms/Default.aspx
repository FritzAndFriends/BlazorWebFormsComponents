<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="BeforeWebForms._Default" %>

<asp:Content ID="BodyContent" ContentPlaceHolderID="MainContent" runat="server">

    <h1>Before Web Forms</h1>

    <p>
        This is a project that contains individual pages for the BlazorWebFormsComponents developers to use 
        to test the existing controls and see how they behave.  We will use this project to determine how best to build
        our matching Blazor components.
    </p>

    <div class="row">

        <div class="col-md-3">
            <h3>Data Controls</h3>
            <ul>
                <li>Chart(?)                   </li>
                <li>DataList                 </li>
                <li>DataPager             </li>
                <li>DetailsView           </li>
                <li>FormView                </li>
                <li>GridView                </li>
                <li><a href="/ControlSamples/ListView">ListView</a></li>
                <li>Repeater                 </li>

            </ul>
        </div>

        <div class="col-md-3">
            <h3>Validation Controls</h3>
            <ul>
                
    <li>CompareValidator                                   </li>
    <li>CustomValidator                                      </li>
    <li>RangeValidator                                         </li>
    <li>RegularExpressionValidator(?)         </li>
    <li>RequiredFieldValidator                        </li>
    <li>ValidationSummary                                 </li>

            </ul>
        </div>           

        <div class="col-md-3">
            <h3>Navigation Controls</h3>
            <ul>
            </ul>
        </div>

        <div class="col-md=3">
                <h3>Login Controls</h3>
            <ul>

            </ul>
        </div>

    </div>
 
</asp:Content>
