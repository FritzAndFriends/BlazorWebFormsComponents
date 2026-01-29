# Button

The **Button** component displays a push button control that allows users to trigger actions. It may seem strange that we have a Button component when there already is an HTML button and Blazor has features that enable C# interactions with that button, but we need to activate other features that were once present in Web Forms, such as `OnCommand` event bubbling, `OnClientClick` JavaScript execution, and `CausesValidation` support.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.button?view=netframework-4.8

## Features Supported in Blazor

- `Text` - the text displayed on the button
- `OnClick` - event handler triggered when button is clicked
- `OnClientClick` - JavaScript code to execute on client-side click
- `OnCommand` - event handler with event bubbling, receives `CommandEventArgs` with `CommandName` and `CommandArgument`
- `CommandName` - the command name passed to `OnCommand` event
- `CommandArgument` - the command argument passed to `OnCommand` event
- `CausesValidation` - controls whether form validation is triggered on click (default: `true`)
- `Enabled` - enables or disables the button
- `Visible` - controls button visibility
- `ToolTip` - tooltip text displayed on hover
- All style properties (`BackColor`, `ForeColor`, `BorderColor`, `BorderStyle`, `BorderWidth`, `CssClass`, `Width`, `Height`, `Font`)

### Blazor Notes

- The `OnCommand` event uses a `CommandEventArgs` class that contains `CommandName` and `CommandArgument` properties
- When `CommandName` is set, clicking the button triggers `OnCommand` instead of `OnClick`
- Event bubbling is supported for container components that need to handle commands from child buttons

## Web Forms Features NOT Supported

- **PostBackUrl** - Not supported; Blazor uses component events instead of postbacks to different pages
- **UseSubmitBehavior** - Not supported; Blazor buttons trigger click events and you can inspect the form regardless
- **ValidationGroup** - Not yet implemented; use EditForm validation instead
- **AccessKey** - Use HTML `accesskey` attribute directly if needed

## Web Forms Declarative Syntax

```html
<asp:Button
    AccessKey="string"
    BackColor="color name|#dddddd"
    BorderColor="color name|#dddddd"
    BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|Groove|Ridge|
        Inset|Outset"
    BorderWidth="size"
    CausesValidation="True|False"
    CommandArgument="string"
    CommandName="string"
    CssClass="string"
    Enabled="True|False"
    EnableTheming="True|False"
    EnableViewState="True|False"
    Font-Bold="True|False"
    Font-Italic="True|False"
    Font-Names="string"
    Font-Overline="True|False"
    Font-Size="string|Smaller|Larger|XX-Small|X-Small|Small|Medium|
        Large|X-Large|XX-Large"
    Font-Strikeout="True|False"
    Font-Underline="True|False"
    ForeColor="color name|#dddddd"
    Height="size"
    ID="string"
    OnClick="Click event handler"
    OnClientClick="string"
    OnCommand="Command event handler"
    OnDataBinding="DataBinding event handler"
    OnDisposed="Disposed event handler"
    OnInit="Init event handler"
    OnLoad="Load event handler"
    OnPreRender="PreRender event handler"
    OnUnload="Unload event handler"
    PostBackUrl="uri"
    runat="server"
    SkinID="string"
    Style="string"
    TabIndex="integer"
    Text="string"
    ToolTip="string"
    UseSubmitBehavior="True|False"
    ValidationGroup="string"
    Visible="True|False"
    Width="size"
/>
```

## Blazor Razor Syntax

### Basic Button with Click Event

```razor
<Button Text="Click Me" OnClick="HandleClick" />

@code {
    void HandleClick()
    {
        // Handle the click
    }
}
```

### Button with Command

```razor
<Button Text="Save" 
        CommandName="Save" 
        CommandArgument="@itemId" 
        OnCommand="HandleCommand" />

@code {
    private string itemId = "123";

    void HandleCommand(CommandEventArgs args)
    {
        var command = args.CommandName;    // "Save"
        var argument = args.CommandArgument; // "123"
    }
}
```

### Button with JavaScript Click

```razor
<Button Text="Alert" OnClientClick="alert('Hello World!')" />
```

### Styled Button

```razor
<Button Text="Styled Button"
        BackColor="WebColor.Blue"
        ForeColor="WebColor.White"
        Font_Bold="true"
        CssClass="my-button-class" />
```

### Disabled Button

```razor
<Button Text="Cannot Click" Enabled="false" />
```

### Button with Validation Control

```razor
<EditForm Model="@model" OnValidSubmit="HandleSubmit">
    <TextBox @bind-Text="model.Name" />
    <RequiredFieldValidator ControlToValidate="..." Text="Required" />
    
    @* This button triggers validation *@
    <Button Text="Submit" CausesValidation="true" />
    
    @* This button skips validation *@
    <Button Text="Cancel" CausesValidation="false" OnClick="HandleCancel" />
</EditForm>
```

## HTML Output

**Web Forms Input:**
```html
<asp:Button ID="btnSubmit" Text="Submit" CssClass="btn" runat="server" />
```

**Rendered HTML:**
```html
<button type="submit" class="btn">Submit</button>
```

**Styled Button Input:**
```razor
<Button Text="Styled" BackColor="WebColor.Blue" ForeColor="WebColor.White" />
```

**Rendered HTML:**
```html
<button type="submit" style="background-color:Blue;color:White;">Styled</button>
```

## Migration Notes

When migrating from Web Forms to Blazor:

1. **Remove `asp:` prefix** - Change `<asp:Button>` to `<Button>`
2. **Remove `runat="server"`** - Not needed in Blazor
3. **Remove `ID` attribute** - Use `@ref` if you need a reference to the component
4. **Update event handler syntax** - Change `OnClick="btnSubmit_Click"` to `OnClick="HandleClick"`
5. **Replace `PostBackUrl`** - Use navigation or component state instead
6. **Update `OnCommand` handlers** - The signature changes from `(object sender, CommandEventArgs e)` to `(CommandEventArgs args)`

### Before (Web Forms)

```html
<asp:Button ID="btnSave" 
            Text="Save" 
            OnClick="btnSave_Click" 
            CssClass="btn btn-primary"
            runat="server" />
```

```csharp
protected void btnSave_Click(object sender, EventArgs e)
{
    // Handle click
}
```

### After (Blazor)

```razor
<Button Text="Save" 
        OnClick="HandleSave" 
        CssClass="btn btn-primary" />

@code {
    void HandleSave()
    {
        // Handle click
    }
}
```

## Examples

### Basic Click Handler

```razor
<Button Text="Click Me!" OnClick="OnClick" />

<span style="font-weight: bold">@message</span>

@code {
    private string message = "Not clicked yet!";

    void OnClick()
    {
        message = "I've been clicked!";
    }
}
```

### Command Pattern with Multiple Buttons

```razor
<Button Text="Edit" CommandName="Edit" CommandArgument="@item.Id" OnCommand="HandleCommand" />
<Button Text="Delete" CommandName="Delete" CommandArgument="@item.Id" OnCommand="HandleCommand" />

<p>Last action: @lastAction</p>

@code {
    private string lastAction = "None";

    void HandleCommand(CommandEventArgs args)
    {
        lastAction = $"Command '{args.CommandName}' on item {args.CommandArgument}";
    }
}
```

### Styled Buttons

```razor
@using static BlazorWebFormsComponents.WebColor

<Button Text="Blue Button" BackColor="Blue" ForeColor="White" />
<Button Text="Red Bold Button" BackColor="Red" ForeColor="Yellow" Font_Bold="true" />
<Button Text="Custom Class" CssClass="rounded-corners" />
```

### JavaScript Integration

```razor
<Button Text="Show Alert" OnClientClick="alert('Hello from JavaScript!')" />
<Button Text="Confirm" OnClientClick="return confirm('Are you sure?')" OnClick="HandleConfirmed" />
```

## See Also

- [LinkButton](LinkButton.md) - A button that renders as a hyperlink
- [ImageButton](ImageButton.md) - A button that displays an image
- [RequiredFieldValidator](../ValidationControls/RequiredFieldValidator.md) - Validate required fields
- [Live Demo](https://blazorwebformscomponents.azurewebsites.net/ControlSamples/Button) - Interactive Button samples
