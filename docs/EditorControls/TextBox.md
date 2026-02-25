# TextBox

The **TextBox** component displays a single-line text box, multi-line text area, or specialized HTML5 input control (password, email, number, date, etc.) for user input.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.textbox?view=netframework-4.8

## Features Supported in Blazor

- Single-line text input (`<input type="text">`)
- Multi-line text area (`<textarea>`)
- Password input (`<input type="password">`)
- HTML5 input types (email, number, date, url, tel, etc.)
- Two-way data binding with `@bind-Text`
- MaxLength validation
- ReadOnly and Enabled/Disabled states
- Rows and Columns for sizing
- Placeholder text
- All style properties (BackColor, ForeColor, BorderColor, CssClass, Width, Height, etc.)
- `ToolTip` - tooltip text displayed on hover (renders as `title` attribute)
- TabIndex support

## Web Forms Features NOT Supported

- **AutoPostBack** - Not needed in Blazor; use event handlers instead
- **AutoComplete** - Handled by browser settings
- **TextChanged server event** - Use `@bind-Text` or `@onchange` event in Blazor
- **Wrap** - Use CSS styling instead

## Web Forms Declarative Syntax

```html
<asp:TextBox
    AccessKey="string"
    AutoCompleteType="None|Disabled|Search|FirstName|LastName|MiddleName|
        NamePrefix|NameSuffix|FullName|Nickname|Email|HomePhone|WorkPhone|
        CellPhone|HomeStreetAddress|HomeCity|HomeState|HomeZipCode|
        HomeCountryRegion|WorkStreetAddress|WorkCity|WorkState|WorkZipCode|
        WorkCountryRegion|Gender|Birthday|Occupation|BusinessUrl|
        Company|Department|DisplayName|JobTitle|Notes"
    AutoPostBack="True|False"
    BackColor="color name|#dddddd"
    BorderColor="color name|#dddddd"
    BorderStyle="NotSet|None|Dotted|Dashed|Solid|Double|Groove|Ridge|
        Inset|Outset"
    BorderWidth="size"
    Columns="integer"
    CssClass="string"
    Enabled="True|False"
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
    MaxLength="integer"
    OnTextChanged="TextChanged event handler"
    Placeholder="string"
    ReadOnly="True|False"
    Rows="integer"
    runat="server"
    TabIndex="integer"
    Text="string"
    TextMode="SingleLine|MultiLine|Password|Color|Date|DateTime|
        DateTimeLocal|Email|Month|Number|Range|Search|Phone|Time|Url|Week"
    ToolTip="string"
    Visible="True|False"
    Width="size"
    Wrap="True|False"
/>
```

## Blazor Razor Syntax

### Single-Line Text Input

```razor
<TextBox Text="Enter your name" CssClass="form-control" />
```

### Two-Way Data Binding

```razor
<TextBox @bind-Text="userName" />

@code {
    private string userName = "John Doe";
}
```

### Password Input

```razor
<TextBox TextMode="TextBoxMode.Password" Placeholder="Enter password" />
```

### Multi-Line Text Area

```razor
<TextBox TextMode="TextBoxMode.MultiLine" 
         Rows="5" 
         Columns="40" 
         @bind-Text="comments" />
```

### HTML5 Input Types

```razor
<!-- Email -->
<TextBox TextMode="TextBoxMode.Email" Placeholder="email@example.com" />

<!-- Number -->
<TextBox TextMode="TextBoxMode.Number" />

<!-- Date -->
<TextBox TextMode="TextBoxMode.Date" />

<!-- URL -->
<TextBox TextMode="TextBoxMode.Url" Placeholder="https://example.com" />

<!-- Phone -->
<TextBox TextMode="TextBoxMode.Phone" Placeholder="(555) 123-4567" />
```

### ReadOnly and Disabled

```razor
<TextBox Text="Read only text" ReadOnly="true" />
<TextBox Text="Disabled text" Enabled="false" />
```

### With MaxLength Validation

```razor
<TextBox MaxLength="10" Placeholder="Max 10 characters" />
```

### Styled TextBox

```razor
<TextBox Text="Styled input"
         BackColor="WebColor.LightYellow"
         ForeColor="WebColor.Navy"
         BorderColor="WebColor.Blue"
         BorderWidth="Unit.Pixel(2)"
         BorderStyle="BorderStyle.Solid"
         Width="Unit.Pixel(300)" />
```

## HTML Output

### Single-Line TextBox
**Web Forms Input:**
```html
<asp:TextBox ID="txtName" Text="John" CssClass="form-control" runat="server" />
```

**Rendered HTML:**
```html
<input type="text" value="John" class="form-control" />
```

### Password TextBox
**Web Forms Input:**
```html
<asp:TextBox ID="txtPass" TextMode="Password" runat="server" />
```

**Rendered HTML:**
```html
<input type="password" />
```

### Multi-Line TextBox
**Web Forms Input:**
```html
<asp:TextBox ID="txtComments" TextMode="MultiLine" Rows="5" Columns="40" runat="server" />
```

**Rendered HTML:**
```html
<textarea rows="5" cols="40"></textarea>
```

## Migration Notes

When migrating from Web Forms to Blazor:

1. **Remove `runat="server"`** - Not needed in Blazor
2. **Remove `AutoPostBack`** - Use `@bind-Text` or event handlers
3. **Replace `OnTextChanged` with `@bind-Text`** - For real-time updates
4. **Update `TextMode` references** - Use `TextBoxMode.` enum prefix (e.g., `TextBoxMode.Password`)
5. **Use HTML5 input types** - Take advantage of new types like Email, Number, Date, etc.

## See Also

- [Label](Label.md) - Display static text
- [Button](Button.md) - Trigger actions
- [RequiredFieldValidator](../ValidationControls/RequiredFieldValidator.md) - Validate required fields
