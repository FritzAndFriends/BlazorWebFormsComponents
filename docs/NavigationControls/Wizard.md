# Wizard

The Wizard component provides a multi-step navigation UI for collecting related data across sequential steps. It guides users through a series of related tasks with automatic button state management based on step position.

Original Microsoft documentation: [Wizard Class (System.Web.UI.WebControls)](https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.wizard?view=netframework-4.8)

## Features Supported in Blazor

- **Multi-Step Navigation** – Sequential flow with automatic Next/Previous/Finish button state management
- **Step Definitions** – `WizardStep` child components with `Title`, `StepType`, and `AllowReturn` control
- **Automatic Type Detection** – `WizardStepType` auto-determines button layout based on step position (Start → Step → Finish → Complete)
- **Sidebar Navigation** – Optional sidebar showing all step titles with click-to-navigate (respects `AllowReturn`)
- **Programmatic Control** – `ActiveStepIndex` and `ActiveStepIndexChanged` for two-way binding
- **Custom Button Text** – Per-step button labels (`StartNextButtonText`, `StepNextButtonText`, `FinishPreviousButtonText`, etc.)
- **Navigation Events** – `OnNextButtonClick`, `OnPreviousButtonClick`, `OnFinishButtonClick`, `OnCancelButtonClick`, `OnSideBarButtonClick`, `OnActiveStepChanged`
- **Navigation Cancellation** – `WizardNavigationEventArgs.Cancel` property allows event handlers to prevent step transitions
- **Destination URLs** – `FinishDestinationPageUrl` and `CancelDestinationPageUrl` for post-completion navigation
- **Custom Templates** – `HeaderTemplate`, `SideBarTemplate`, `StartNavigationTemplate`, `StepNavigationTemplate`, and `FinishNavigationTemplate` replace the default markup for their respective regions
- **Styling** – `NavigationButtonStyle`, `SideBarStyle`, `SideBarButtonStyle`, `HeaderStyle`, `StepStyle`, `NavigationStyle` (all `TableItemStyle`)
- **Base Styles** – Inherits from `BaseStyledComponent`: `BackColor`, `ForeColor`, `BorderColor`, `CssClass`, `Font`, `Height`, `Width`

## Web Forms Features NOT Supported

- `DataList` property (data-bound step generation) – Define steps declaratively in Blazor
- Server-side `WizardStepCollection` manipulation at runtime – Steps must be declared at component load time
- Design-time support and smart tags – Visual Studio features not applicable to Blazor

## Wizard Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ActiveStepIndex` | int | 0 | The zero-based index of the currently active step. Use with `ActiveStepIndexChanged` for two-way binding. |
| `ActiveStepIndexChanged` | `EventCallback<int>` | — | Callback fired when the active step changes (Next, Previous, or sidebar navigation). |
| `DisplaySideBar` | bool | true | Show/hide the sidebar with step titles and navigation links. |
| `DisplayCancelButton` | bool | false | Show/hide the Cancel button in the navigation area. |
| `HeaderText` | string | null | Optional text or content displayed in the header area above the active step. |
| `FinishDestinationPageUrl` | string | null | URL to navigate to after Finish button is clicked. If empty, navigates to Complete step if present. |
| `CancelDestinationPageUrl` | string | null | URL to navigate to after Cancel button is clicked. |

### Button Text Properties

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `StartNextButtonText` | string | "Next" | Text for Next button on the first step. |
| `StepNextButtonText` | string | "Next" | Text for Next button on middle steps. |
| `StepPreviousButtonText` | string | "Previous" | Text for Previous button on middle steps. |
| `FinishButtonText` | string | "Finish" | Text for Finish button on the last non-Complete step. |
| `FinishPreviousButtonText` | string | "Previous" | Text for Previous button on the last non-Complete step. |
| `FinishCompleteButtonText` | string | "Finish" | Text rendered on the Finish button. A non-default value overrides `FinishButtonText`; otherwise existing `FinishButtonText` customizations continue to work. |
| `CancelButtonText` | string | "Cancel" | Text for Cancel button (shown if `DisplayCancelButton="true"`). |

### Style Properties

All inherit from `BaseStyledComponent` (`CssClass`, `BackColor`, `ForeColor`, `BorderColor`, `Font`, `Height`, `Width`).

| Property | Type | Description |
|----------|------|-------------|
| `NavigationButtonStyle` | `TableItemStyle` | Style applied to individual navigation buttons (Next, Previous, Finish, Cancel). |
| `SideBarStyle` | `TableItemStyle` | Style applied to the sidebar container (when `DisplaySideBar="true"`). |
| `SideBarButtonStyle` | `TableItemStyle` | Style applied to clickable step links in the sidebar. |
| `HeaderStyle` | `TableItemStyle` | Style applied to the header row (if `HeaderText` or `HeaderTemplate` is set). |
| `StepStyle` | `TableItemStyle` | Style applied to the active step content area. |
| `NavigationStyle` | `TableItemStyle` | Style applied to the navigation row (button container). |

## WizardStep Properties

Nest `<WizardStep>` components inside the `<WizardSteps>` template parameter.

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Title` | string | null | Display name for this step (shown in sidebar and as step label). |
| `StepType` | `WizardStepType` | Auto | Determines button layout for this step. See [Step Type Behavior](#step-type-behavior). |
| `AllowReturn` | bool | true | If false, users cannot navigate back to this step via Previous or sidebar. Useful for "no backtracking" workflows. |
| `ChildContent` | `RenderFragment` | — | Content rendered when this step is active. |

## WizardStepType Enum

| Value | Position | Buttons Shown | Behavior |
|-------|----------|---------------|----------|
| `Auto` | *Auto-determined* | *Depends on position* | Default. Wizard automatically selects type based on step's position in sequence. |
| `Start` | First step | Next button only | Initial step with forward navigation only. |
| `Step` | Middle steps | Previous + Next buttons | Intermediate step with bidirectional navigation. |
| `Finish` | Last non-Complete | Previous + Finish buttons | Final data-entry step before completion. Finish button replaces Next. |
| `Complete` | After Finish | No buttons | Confirmation/summary step shown after Finish clicked. No navigation buttons rendered. |

## Navigation Events

All navigation events receive `WizardNavigationEventArgs` with `Cancel` and step index properties.

| Event | Signature | When Fired | Cancellable |
|-------|-----------|-----------|------------|
| `OnNextButtonClick` | `EventCallback<WizardNavigationEventArgs>` | User clicks Next button | Yes – set `args.Cancel = true` to stay on current step |
| `OnPreviousButtonClick` | `EventCallback<WizardNavigationEventArgs>` | User clicks Previous button | Yes – set `args.Cancel = true` to stay on current step |
| `OnFinishButtonClick` | `EventCallback<WizardNavigationEventArgs>` | User clicks Finish button | Yes – set `args.Cancel = true` to prevent completion |
| `OnSideBarButtonClick` | `EventCallback<WizardNavigationEventArgs>` | User clicks step in sidebar | Yes – set `args.Cancel = true` to cancel jump |
| `OnCancelButtonClick` | `EventCallback<EventArgs>` | User clicks Cancel button | No – always proceeds to `CancelDestinationPageUrl` or stays on current step |
| `OnActiveStepChanged` | `EventCallback<EventArgs>` | Step successfully changed (after all OnXxxButtonClick events complete) | No – informational only |

### WizardNavigationEventArgs

```csharp
public class WizardNavigationEventArgs : EventArgs
{
    public int CurrentStepIndex { get; }      // Zero-based index before navigation
    public int NextStepIndex { get; }         // Zero-based index after navigation
    public bool Cancel { get; set; }          // Set to true to prevent navigation
}
```

## Web Forms Declarative Syntax

```html
<asp:Wizard ID="Wizard1" runat="server"
    ActiveStepIndex="0"
    DisplaySideBar="true"
    DisplayCancelButton="false"
    HeaderText="Registration"
    OnFinishButtonClick="Wizard1_FinishButtonClick"
    OnActiveStepChanged="Wizard1_ActiveStepChanged">
    <WizardSteps>
        <asp:WizardStep ID="Step1" Title="Personal Info" runat="server">
            <p>Name: <asp:TextBox runat="server" /></p>
        </asp:WizardStep>
        <asp:WizardStep ID="Step2" Title="Address" runat="server">
            <p>Street: <asp:TextBox runat="server" /></p>
        </asp:WizardStep>
        <asp:WizardStep ID="ReviewStep" Title="Review" StepType="Finish" runat="server">
            <p>Please confirm your information and click Finish.</p>
        </asp:WizardStep>
        <asp:WizardStep ID="CompleteStep" StepType="Complete" runat="server">
            <p>Thank you! Your registration is complete.</p>
        </asp:WizardStep>
    </WizardSteps>
</asp:Wizard>

<script runat="server">
    private void Wizard1_FinishButtonClick(object sender, WizardNavigationEventArgs e)
    {
        // Validate and save form data
    }
    
    private void Wizard1_ActiveStepChanged(object sender, EventArgs e)
    {
        // React to step change
    }
</script>
```

## Blazor Syntax

```razor
@using BlazorWebFormsComponents.Enums

<Wizard @ref="wizard"
        ActiveStepIndex="@activeStepIndex"
        ActiveStepIndexChanged="(i) => OnActiveStepIndexChanged(i)"
        HeaderText="Registration"
        DisplaySideBar="true"
        DisplayCancelButton="true"
        OnNextButtonClick="HandleNextButtonClick"
        OnPreviousButtonClick="HandlePreviousButtonClick"
        OnFinishButtonClick="HandleFinishButtonClick"
        OnCancelButtonClick="HandleCancelButtonClick"
        OnActiveStepChanged="HandleActiveStepChanged">
    <WizardSteps>
        <WizardStep Title="Personal Info" StepType="WizardStepType.Auto">
            <p>Name: <input type="text" @bind="formData.Name" /></p>
        </WizardStep>
        <WizardStep Title="Address" StepType="WizardStepType.Auto">
            <p>Street: <input type="text" @bind="formData.Street" /></p>
        </WizardStep>
        <WizardStep Title="Review" StepType="WizardStepType.Finish">
            <p>Please confirm your information and click Finish.</p>
        </WizardStep>
        <WizardStep Title="Complete" StepType="WizardStepType.Complete">
            <p>Thank you! Your registration is complete.</p>
        </WizardStep>
    </WizardSteps>
</Wizard>

@code {
    private Wizard wizard;
    private int activeStepIndex;
    
    private class FormData
    {
        public string Name { get; set; }
        public string Street { get; set; }
    }
    
    private FormData formData = new();
    
    private void OnActiveStepIndexChanged(int index)
    {
        activeStepIndex = index;
    }
    
    private void HandleNextButtonClick(WizardNavigationEventArgs args)
    {
        // Validate current step before allowing next
        if (activeStepIndex == 0 && string.IsNullOrWhiteSpace(formData.Name))
        {
            args.Cancel = true; // Prevent navigation
        }
    }
    
    private void HandlePreviousButtonClick(WizardNavigationEventArgs args)
    {
        // Optional: prevent backing out
    }
    
    private void HandleFinishButtonClick(WizardNavigationEventArgs args)
    {
        // Save form data
    }
    
    private void HandleCancelButtonClick(EventArgs args)
    {
        // Clean up if needed
    }
    
    private void HandleActiveStepChanged(EventArgs args)
    {
        // Respond to step change
    }
}
```

## Usage Notes

### Step Type Behavior

When `StepType` is set to `Auto` (the default), the wizard determines navigation buttons automatically based on step position:

| Position | Effective Type | Buttons Shown | Use Case |
|----------|---------------|---------------|----------|
| First step | `Start` | Next only | Initial step with forward navigation only. |
| Middle steps | `Step` | Previous + Next | Intermediate steps with bidirectional navigation. |
| Last non-Complete step | `Finish` | Previous + Finish | Final data-entry step. Finish replaces Next button. |
| `Complete` step (after Finish) | `Complete` | *(none)* | Summary/confirmation shown after Finish. No navigation buttons. |

**Example auto-detection:**
```razor
<WizardSteps>
    <WizardStep Title="Name" />              <!-- Auto → Start (first) -->
    <WizardStep Title="Address" />          <!-- Auto → Step (middle) -->
    <WizardStep Title="Review" />           <!-- Auto → Finish (last non-Complete) -->
    <WizardStep Title="Complete" />         <!-- Auto → Complete (explicit step) -->
</WizardSteps>
```

### AllowReturn Behavior

Set `AllowReturn="false"` on a step to prevent users from navigating back to it. The Previous button and sidebar will skip over such steps.

```razor
<!-- User cannot return to Step 1 after moving forward -->
<WizardStep Title="Billing Info" AllowReturn="false">
    <!-- Form content -->
</WizardStep>
```

**Sidebar behavior:** Steps with `AllowReturn="false"` and index < current step show as plain text (not clickable).

**Previous button behavior:** Repeatedly calls Previous until finding a step with `AllowReturn="true"` or reaching step 0.

### Cancelling Navigation

Event handlers receive `WizardNavigationEventArgs` with a `Cancel` property. Set `Cancel = true` to prevent the navigation action:

```csharp
private void HandleNextButtonClick(WizardNavigationEventArgs args)
{
    // Validate form before allowing next
    if (!IsFormValid())
    {
        args.Cancel = true;  // Stay on current step
    }
}
```

!!! tip
    Use `WizardNavigationEventArgs.CurrentStepIndex` and `NextStepIndex` to determine direction or implement multi-step validation logic.

### Two-Way Binding

Use `ActiveStepIndex` with `ActiveStepIndexChanged` for two-way binding of the current step:

```razor
<Wizard ActiveStepIndex="@step" ActiveStepIndexChanged="(i) => step = i">
    <!-- steps -->
</Wizard>

@code {
    private int step = 0;
}
```

Programmatically change steps by updating the bound property:
```csharp
step = 2;  // Jump to step 2 (bypassing validation, if needed)
```

### Custom Templates

Replace the default sidebar or step-specific button layouts with custom templates. `SideBarTemplate` replaces the built-in step links, and each navigation template replaces the default button row for its matching step type (`Start`, `Step`, or `Finish`):

```razor
<Wizard>
    <HeaderTemplate>
        <div style="background-color: #f0f0f0; padding: 10px;">
            <h2>Custom Header</h2>
        </div>
    </HeaderTemplate>
    
    <SideBarTemplate>
        <div style="display: flex; flex-direction: column; gap: 5px;">
            @for (int i = 0; i < Wizard.WizardStepsList.Count; i++)
            {
                <button @onclick="() => HandleSideBarNav(i)">
                    @Wizard.WizardStepsList[i].Title
                </button>
            }
        </div>
    </SideBarTemplate>
    
    <StartNavigationTemplate>
        <button @onclick="HandleNext">Go Forward</button>
    </StartNavigationTemplate>
    
    <WizardSteps>
        <!-- steps -->
    </WizardSteps>
</Wizard>
```

### Navigation Destination URLs

Use `FinishDestinationPageUrl` or `CancelDestinationPageUrl` to redirect after completion:

```razor
<!-- Redirect to thank-you page after Finish -->
<Wizard FinishDestinationPageUrl="/thank-you"
        CancelDestinationPageUrl="/">
    <!-- steps -->
</Wizard>
```

If `FinishDestinationPageUrl` is empty and there's a Complete step, the wizard navigates to that step instead of redirecting.

!!! warning
    If both a Complete step exists AND `FinishDestinationPageUrl` is set, the URL takes precedence.

### HTML Output

The Wizard renders as a nested table structure with the following layout:

```html
<table style="border-collapse:collapse;">
    <tbody>
        <tr>
            <!-- Optional sidebar -->
            <td>
                <a href="#">Step 1</a><br/>
                <span style="font-weight:bold;">Step 2 (active)</span><br/>
                <a href="#">Step 3</a><br/>
            </td>
            
            <!-- Main content -->
            <td>
                <table style="width:100%;height:100%;">
                    <tbody>
                        <!-- Optional header -->
                        <tr>
                            <td>Header content</td>
                        </tr>
                        
                        <!-- Active step content -->
                        <tr>
                            <td>Active step markup</td>
                        </tr>
                        
                        <!-- Navigation buttons -->
                        <tr>
                            <td align="right">
                                <input type="button" value="Previous" />
                                <input type="button" value="Next" />
                            </td>
                        </tr>
                    </tbody>
                </table>
            </td>
        </tr>
    </tbody>
</table>
```

!!! note
    The Complete step renders no buttons. The sidebar is hidden when viewing a Complete step.

## Examples

### Basic Multi-Step Form

A simple 3-step form with validation on the next button:

```razor
@using BlazorWebFormsComponents.Enums

<Wizard @ref="basicWizard"
        ActiveStepIndex="@currentStep"
        ActiveStepIndexChanged="(i) => currentStep = i"
        OnNextButtonClick="HandleNextClick"
        OnFinishButtonClick="HandleFinishClick">
    <WizardSteps>
        <WizardStep Title="Name">
            <div>
                <label>Your Name:</label>
                <input type="text" @bind="name" />
            </div>
        </WizardStep>
        <WizardStep Title="Email">
            <div>
                <label>Email Address:</label>
                <input type="email" @bind="email" />
            </div>
        </WizardStep>
        <WizardStep Title="Confirm" StepType="WizardStepType.Finish">
            <div>
                <p>Name: @name</p>
                <p>Email: @email</p>
                <p>Click Finish to complete registration.</p>
            </div>
        </WizardStep>
        <WizardStep Title="Thank You" StepType="WizardStepType.Complete">
            <p>Thank you for registering! Check your email for confirmation.</p>
        </WizardStep>
    </WizardSteps>
</Wizard>

@code {
    private Wizard basicWizard;
    private int currentStep;
    private string name = "";
    private string email = "";
    
    private void HandleNextClick(WizardNavigationEventArgs args)
    {
        // Validate current step
        if (args.CurrentStepIndex == 0 && string.IsNullOrWhiteSpace(name))
        {
            args.Cancel = true;  // Stay on Name step
        }
        else if (args.CurrentStepIndex == 1 && string.IsNullOrWhiteSpace(email))
        {
            args.Cancel = true;  // Stay on Email step
        }
    }
    
    private void HandleFinishClick(WizardNavigationEventArgs args)
    {
        // Save registration
        // Navigate to Thank You step
    }
}
```

### Wizard with AllowReturn Control

Prevent users from returning to billing info after moving forward:

```razor
<Wizard ActiveStepIndex="@step"
        ActiveStepIndexChanged="(i) => step = i"
        DisplaySideBar="true">
    <WizardSteps>
        <WizardStep Title="Shipping" AllowReturn="true">
            <input type="text" placeholder="Address" />
        </WizardStep>
        <WizardStep Title="Billing" AllowReturn="false">
            <!-- Once past this step, user cannot return to edit billing -->
            <input type="text" placeholder="Card Number" />
        </WizardStep>
        <WizardStep Title="Review" StepType="WizardStepType.Finish">
            <p>Review your order and click Finish to confirm.</p>
        </WizardStep>
        <WizardStep Title="Order Placed" StepType="WizardStepType.Complete">
            <p>Your order has been placed!</p>
        </WizardStep>
    </WizardSteps>
</Wizard>

@code {
    private int step = 0;
}
```

### Wizard with Custom Navigation

Disable/enable buttons and customize button text per step:

```razor
<Wizard ActiveStepIndex="@step"
        ActiveStepIndexChanged="(i) => step = i"
        StartNextButtonText="Start Survey"
        StepNextButtonText="Next Question"
        StepPreviousButtonText="Back"
        FinishButtonText="Submit Survey"
        DisplayCancelButton="true"
        OnNextButtonClick="ValidateStep"
        OnCancelButtonClick="CancelSurvey">
    <WizardSteps>
        <WizardStep Title="Question 1">
            <fieldset>
                <legend>Do you like Blazor?</legend>
                <label><input type="radio" name="q1" value="yes" /> Yes</label>
                <label><input type="radio" name="q1" value="no" /> No</label>
            </fieldset>
        </WizardStep>
        <WizardStep Title="Question 2">
            <fieldset>
                <legend>Would you recommend it?</legend>
                <label><input type="radio" name="q2" value="yes" /> Yes</label>
                <label><input type="radio" name="q2" value="no" /> No</label>
            </fieldset>
        </WizardStep>
        <WizardStep Title="Complete" StepType="WizardStepType.Finish">
            <p>Thank you for completing the survey!</p>
        </WizardStep>
    </WizardSteps>
</Wizard>

@code {
    private int step = 0;
    
    private void ValidateStep(WizardNavigationEventArgs args)
    {
        // Ensure user selected an answer
        if (!IsAnswerSelected(args.CurrentStepIndex))
        {
            args.Cancel = true;
        }
    }
    
    private bool IsAnswerSelected(int stepIndex)
    {
        // Check for selected radio button, etc.
        return true;
    }
    
    private void CancelSurvey(EventArgs args)
    {
        // Clear form
    }
}
```

### Wizard with Sidebar and Header

Full-featured wizard with custom header and sidebar navigation:

```razor
<Wizard ActiveStepIndex="@step"
        ActiveStepIndexChanged="(i) => step = i"
        HeaderText="Product Registration"
        DisplaySideBar="true"
        DisplayCancelButton="false"
        FinishDestinationPageUrl="/registration-complete">
    <HeaderTemplate>
        <div style="background: linear-gradient(90deg, #007bff, #0056b3); color: white; padding: 15px; border-radius: 4px; text-align: center;">
            <h2>Register Your Product</h2>
            <p>Step @(step + 1) of 3 - @GetStepTitle(step)</p>
        </div>
    </HeaderTemplate>
    
    <SideBarTemplate>
        <div style="padding: 10px; background: #f8f9fa; border-right: 1px solid #ddd;">
            <strong>Steps:</strong>
            <ol style="margin: 10px 0;">
                @for (int i = 0; i < 3; i++)
                {
                    var idx = i;
                    var isActive = i == step;
                    var isBefore = i < step;
                    <li style="margin: 5px 0; @(isActive ? "font-weight:bold; color:#007bff;" : "")">
                        @if (isActive)
                        {
                            <span>@GetStepTitle(i) ✓</span>
                        }
                        else if (isBefore)
                        {
                            <a href="#" style="text-decoration: underline;">@GetStepTitle(i)</a>
                        }
                        else
                        {
                            <span>@GetStepTitle(i)</span>
                        }
                    </li>
                }
            </ol>
        </div>
    </SideBarTemplate>
    
    <WizardSteps>
        <WizardStep Title="Product Info">
            <div style="padding: 15px;">
                <label>Product Serial Number:</label>
                <input type="text" @bind="serialNumber" />
            </div>
        </WizardStep>
        <WizardStep Title="Contact Info">
            <div style="padding: 15px;">
                <label>Email:</label>
                <input type="email" @bind="contactEmail" />
            </div>
        </WizardStep>
        <WizardStep Title="Confirm" StepType="WizardStepType.Finish">
            <div style="padding: 15px;">
                <p><strong>Serial:</strong> @serialNumber</p>
                <p><strong>Email:</strong> @contactEmail</p>
            </div>
        </WizardStep>
        <WizardStep Title="Success" StepType="WizardStepType.Complete">
            <div style="padding: 15px; text-align: center; color: green;">
                <p>✓ Registration successful!</p>
            </div>
        </WizardStep>
    </WizardSteps>
</Wizard>

@code {
    private int step = 0;
    private string serialNumber = "";
    private string contactEmail = "";
    
    private string GetStepTitle(int index) => index switch
    {
        0 => "Product Info",
        1 => "Contact Info",
        2 => "Confirm",
        _ => "Done"
    };
}
```

### Wizard with Programmatic Navigation

Use a parent component or service to control wizard navigation:

```razor
<Wizard @ref="wizard"
        ActiveStepIndex="@currentStep"
        ActiveStepIndexChanged="OnStepChanged">
    <WizardSteps>
        <WizardStep Title="Step 1">
            <button @onclick="SkipToStep3">Jump to Step 3</button>
        </WizardStep>
        <WizardStep Title="Step 2">Content</WizardStep>
        <WizardStep Title="Step 3">Content</WizardStep>
    </WizardSteps>
</Wizard>

@code {
    private Wizard wizard;
    private int currentStep = 0;
    
    private void OnStepChanged(int newStep)
    {
        currentStep = newStep;
    }
    
    private void SkipToStep3()
    {
        currentStep = 2;  // Jump directly to Step 3 (index 2)
    }
}
```

## See Also

- [Menu](Menu.md) – Hierarchical navigation control
- [TreeView](TreeView.md) – Tree-structured navigation with expand/collapse
- [SiteMapPath](SiteMapPath.md) – Breadcrumb navigation
