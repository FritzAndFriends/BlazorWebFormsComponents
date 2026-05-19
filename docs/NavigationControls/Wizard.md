# Wizard

The Wizard control provides a multi-step navigation UI for collecting related data across sequential steps.

Original Microsoft documentation: [Wizard Class (System.Web.UI.WebControls)](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.wizard?view=netframework-4.8)

## Features Supported in Blazor

- Multi-step navigation with Next/Previous/Finish buttons
- `ActiveStepIndex` for programmatic step control
- `WizardStep` child components with `Title`, `StepType`, and `AllowReturn`
- `WizardStepType` enum: Auto, Start, Step, Finish, Complete
- Sidebar navigation showing all step titles
- `DisplaySideBar` and `DisplayCancelButton` toggle properties
- Custom button text (`StartNextButtonText`, `StepNextButtonText`, `FinishButtonText`, etc.)
- `FinishDestinationPageUrl` and `CancelDestinationPageUrl` for navigation after finish/cancel
- Templates: `HeaderTemplate`, `SideBarTemplate`, `StartNavigationTemplate`, `StepNavigationTemplate`, `FinishNavigationTemplate`
- Style properties: `NavigationButtonStyle`, `SideBarStyle`, `HeaderStyle`, `StepStyle`, `NavigationStyle`
- Events: `OnActiveStepChanged`, `OnNextButtonClick`, `OnPreviousButtonClick`, `OnFinishButtonClick`, `OnCancelButtonClick`, `OnSideBarButtonClick`
- Cancel support via `WizardNavigationEventArgs.Cancel`

## Web Forms Features NOT Supported

- `DataList` property (binding steps from data source)
- Server-side `WizardStepCollection` manipulation at runtime
- Design-time support and smart tags

## Web Forms Declarative Syntax

```html
<asp:Wizard ID="Wizard1" runat="server"
    ActiveStepIndex="0"
    DisplaySideBar="true"
    HeaderText="Registration"
    OnFinishButtonClick="Wizard1_FinishButtonClick"
    OnActiveStepChanged="Wizard1_ActiveStepChanged">
    <WizardSteps>
        <asp:WizardStep ID="Step1" Title="Personal Info" runat="server">
            <!-- Step content -->
        </asp:WizardStep>
        <asp:WizardStep ID="Step2" Title="Review" StepType="Finish" runat="server">
            <!-- Review content -->
        </asp:WizardStep>
        <asp:WizardStep ID="Complete" StepType="Complete" runat="server">
            <!-- Confirmation content -->
        </asp:WizardStep>
    </WizardSteps>
</asp:Wizard>
```

## Blazor Syntax

```razor
<Wizard ActiveStepIndex="@activeStep"
        ActiveStepIndexChanged="(i) => activeStep = i"
        HeaderText="Registration"
        DisplaySideBar="true"
        OnFinishButtonClick="HandleFinish"
        OnActiveStepChanged="HandleStepChanged">
    <WizardSteps>
        <WizardStep Title="Personal Info">
            <!-- Step content -->
        </WizardStep>
        <WizardStep Title="Review" StepType="WizardStepType.Finish">
            <!-- Review content -->
        </WizardStep>
        <WizardStep Title="Complete" StepType="WizardStepType.Complete">
            <!-- Confirmation content -->
        </WizardStep>
    </WizardSteps>
</Wizard>
```

## Usage Notes

### Step Type Behavior

When `StepType` is set to `Auto` (the default), the wizard determines navigation buttons automatically:

| Position | Effective Type | Buttons Shown |
|----------|---------------|---------------|
| First step | Start | Next only |
| Middle steps | Step | Previous + Next |
| Last non-Complete step | Finish | Previous + Finish |
| Complete step | Complete | None |

### AllowReturn

Set `AllowReturn="false"` on a step to prevent users from navigating back to it. The Previous button will skip over that step.

### Cancelling Navigation

Event handlers receive `WizardNavigationEventArgs` with a `Cancel` property. Set it to `true` to prevent navigation:

```csharp
private void HandleNext(WizardNavigationEventArgs args)
{
    if (!IsFormValid())
    {
        args.Cancel = true;
    }
}
```

### Two-Way Binding

Use `ActiveStepIndex` with `ActiveStepIndexChanged` for two-way binding of the current step:

```razor
<Wizard ActiveStepIndex="@step" ActiveStepIndexChanged="(i) => step = i">
```

## Examples

### Basic Multi-Step Form

```razor
@using BlazorWebFormsComponents.Enums

<Wizard ActiveStepIndex="@_step" ActiveStepIndexChanged="(i) => _step = i"
        OnFinishButtonClick="HandleFinish">
    <WizardSteps>
        <WizardStep Title="Name">
            <label>Your Name:</label>
            <input type="text" @bind="_name" />
        </WizardStep>
        <WizardStep Title="Confirm" StepType="WizardStepType.Finish">
            <p>Hello, @_name! Click Finish to complete.</p>
        </WizardStep>
        <WizardStep Title="Done" StepType="WizardStepType.Complete">
            <p>Thank you!</p>
        </WizardStep>
    </WizardSteps>
</Wizard>

@code {
    private int _step;
    private string _name = "";

    private void HandleFinish(WizardNavigationEventArgs args)
    {
        // Process the form data
    }
}
```
