# CheckBox

The CheckBox component provides a Blazor implementation of the ASP.NET Web Forms CheckBox control, enabling boolean input with an optional text label.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.checkbox?view=netframework-4.8

## Features Supported in Blazor

- `Checked` property for boolean state
- `Text` property for label display
- `TextAlign` property (Left or Right) for label positioning
- Two-way binding with `@bind-Checked`
- `OnCheckedChanged` event handler
- `Enabled` property to disable the checkbox
- Style attributes (BackColor, ForeColor, CssClass, etc.)
- `Visible` property to show/hide the checkbox

## Web Forms Features NOT Supported

- `AutoPostBack` is not supported in Blazor. Use the `OnCheckedChanged` event instead to react to state changes.
- `InputAttributes` property is not implemented
- `LabelAttributes` property is not implemented

## Web Forms Declarative Syntax

```html
<asp:CheckBox
    AccessKey="string"
    AutoPostBack="True|False"
    BackColor="color name|#dddddd"
    BorderColor="color name|#dddddd"
    BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|Groove|Ridge|
        Inset|Outset"
    BorderWidth="size"
    Checked="True|False"
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
    OnCheckedChanged="CheckedChanged event handler"
    OnDataBinding="DataBinding event handler"
    OnDisposed="Disposed event handler"
    OnInit="Init event handler"
    OnLoad="Load event handler"
    OnPreRender="PreRender event handler"
    OnUnload="Unload event handler"
    runat="server"
    SkinID="string"
    Style="string"
    TabIndex="integer"
    Text="string"
    TextAlign="Left|Right"
    ToolTip="string"
    ValidationGroup="string"
    Visible="True|False"
    Width="size"
/>
```

## Blazor Syntax

### Basic CheckBox

```razor
<CheckBox Text="I agree to the terms and conditions" />
```

### Pre-checked CheckBox

```razor
<CheckBox Text="Remember me" Checked="true" />
```

### Two-way Binding

```razor
<CheckBox Text="Subscribe to newsletter" @bind-Checked="isSubscribed" />

@code {
    private bool isSubscribed = false;
}
```

### Text Alignment

By default, the label appears to the right of the checkbox. Use `TextAlign` to position it on the left:

```razor
<CheckBox Text="Label on right (default)" />
<CheckBox Text="Label on left" TextAlign="TextAlign.Left" />
```

### Event Handling

```razor
<CheckBox Text="Notify me" OnCheckedChanged="HandleChange" />

@code {
    private void HandleChange(ChangeEventArgs e)
    {
        bool isChecked = (bool)e.Value;
        // Handle the change
    }
}
```

### Disabled CheckBox

```razor
<CheckBox Text="This option is disabled" Enabled="false" Checked="true" />
```

### Styled CheckBox

```razor
@using static BlazorWebFormsComponents.WebColor

<CheckBox Text="Custom styled" 
          CssClass="my-checkbox" 
          BackColor="LightBlue" 
          ForeColor="Navy" />
```

### CheckBox without Text

When no `Text` is provided, the checkbox renders without a `<span>` wrapper or `<label>`:

```razor
<CheckBox @bind-Checked="option1" />
<CheckBox @bind-Checked="option2" />
<CheckBox @bind-Checked="option3" />
```

## HTML Output

The CheckBox component renders different HTML depending on whether text is provided:

**With Text (TextAlign=Right, default):**
```html
<span class="custom-class" style="...">
    <input id="guid" type="checkbox" />
    <label for="guid">Label text</label>
</span>
```

**With Text (TextAlign=Left):**
```html
<span class="custom-class" style="...">
    <label for="guid">Label text</label>
    <input id="guid" type="checkbox" />
</span>
```

**Without Text:**
```html
<input type="checkbox" class="custom-class" style="..." />
```

## Migration Notes

When migrating from Web Forms to Blazor:

1. Remove the `asp:` prefix and `runat="server"` attribute
2. Replace `AutoPostBack="true"` with the `OnCheckedChanged` event handler
3. Use `@bind-Checked` for two-way data binding instead of reading `Checked` in code-behind
4. The `ID` property is obsolete in Blazor; use `@ref` to get a reference to the component instance

### Before (Web Forms):
```html
<asp:CheckBox ID="chkAgree" 
              Text="I agree" 
              AutoPostBack="true"
              OnCheckedChanged="CheckBox_Changed" 
              runat="server" />
```

### After (Blazor):
```razor
<CheckBox Text="I agree" 
          @bind-Checked="agreed"
          OnCheckedChanged="HandleChange" />
```

## Live Sample

See the CheckBox component in action on the [live samples site](https://blazorwebformscomponents.azurewebsites.net/ControlSamples/CheckBox).
