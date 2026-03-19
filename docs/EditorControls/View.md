# View

The **View** component emulates the ASP.NET Web Forms `<asp:View>` control. A View is a container that holds a group of controls within a **MultiView** component. Only one View is visible at a time within its parent MultiView.

Original Microsoft documentation: https://docs.microsoft.com/en-us/dotnet/api/system.web.ui.webcontrols.view?view=netframework-4.8

## Features Supported in Blazor

- `ChildContent` — arbitrary child content rendered when this View is active
- `OnActivate` — event callback fired when this View becomes active in the MultiView
- `OnDeactivate` — event callback fired when this View is deactivated
- Auto-registration with parent MultiView via cascading parameter
- `Visible` — controlled by parent MultiView's `ActiveViewIndex`

## Web Forms Features NOT Supported

- Command-based navigation (handled by parent MultiView)
- `EnableTheming` / `SkinID` — theming not supported in Blazor

## Web Forms Declarative Syntax

```html
<asp:MultiView ID="MultiView1" ActiveViewIndex="0" runat="server">
    <asp:View ID="View1" runat="server">
        <p>Content for View 1</p>
    </asp:View>
    <asp:View ID="View2" runat="server">
        <p>Content for View 2</p>
    </asp:View>
</asp:MultiView>
```

## Blazor Syntax

A View must always be a child of a MultiView component. Only the active View's content is rendered.

### Basic View Within MultiView

```razor
<MultiView ActiveViewIndex="@activeIndex" OnActiveViewChanged="ViewChanged">
    <View>
        <p>This is View 1</p>
        <Button Text="Go to View 2" OnClick="() => activeIndex = 1" />
    </View>
    <View>
        <p>This is View 2</p>
        <Button Text="Go to View 1" OnClick="() => activeIndex = 0" />
    </View>
</MultiView>

@code {
    private int activeIndex = 0;

    private void ViewChanged(EventArgs e)
    {
        // Handle view change
    }
}
```

### View with Event Callbacks

```razor
<MultiView ActiveViewIndex="@activeIndex">
    <View OnActivate="FirstViewActivated" OnDeactivate="FirstViewDeactivated">
        <p>First View</p>
    </View>
    <View OnActivate="SecondViewActivated" OnDeactivate="SecondViewDeactivated">
        <p>Second View</p>
    </View>
</MultiView>

@code {
    private int activeIndex = 0;

    private void FirstViewActivated(EventArgs e) => Console.WriteLine("First view activated");
    private void FirstViewDeactivated(EventArgs e) => Console.WriteLine("First view deactivated");
    private void SecondViewActivated(EventArgs e) => Console.WriteLine("Second view activated");
    private void SecondViewDeactivated(EventArgs e) => Console.WriteLine("Second view deactivated");
}
```

## HTML Output

View renders **no wrapper HTML elements**. Only the active View's child content is rendered directly.

**Blazor Input:**
```razor
<MultiView ActiveViewIndex="0">
    <View><p>View 1 Content</p></View>
    <View><p>View 2 Content</p></View>
</MultiView>
```

**Rendered HTML (when View 1 is active):**
```html
<p>View 1 Content</p>
```

**Rendered HTML (when View 2 is active):**
```html
<p>View 2 Content</p>
```

## Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ChildContent` | RenderFragment | null | The content to render when this View is active |
| `OnActivate` | EventCallback<EventArgs> | null | Fired when this View becomes the active View in the MultiView |
| `OnDeactivate` | EventCallback<EventArgs> | null | Fired when this View is deactivated (another View becomes active) |

## Migration Notes

When migrating from Web Forms to Blazor:

1. **Remove `asp:` prefix and `runat="server"`** — Change `<asp:View>` to `<View>`
2. **Bind ActiveViewIndex** — Use two-way binding on the parent MultiView's `ActiveViewIndex` instead of command-based navigation
3. **Replace command navigation** — Change button command names (`NextView`, `PrevView`) to direct index binding
4. **Use event callbacks** — Replace any View visibility logic with `OnActivate` and `OnDeactivate` callbacks

### Before (Web Forms)

```html
<asp:MultiView ID="MultiView1" ActiveViewIndex="0" runat="server">
    <asp:View ID="View1" runat="server">
        <p>Step 1</p>
        <asp:Button ID="btn1" CommandName="NextView" Text="Next" runat="server" />
    </asp:View>
    <asp:View ID="View2" runat="server">
        <p>Step 2</p>
        <asp:Button ID="btn2" CommandName="PrevView" Text="Previous" runat="server" />
    </asp:View>
</asp:MultiView>
```

### After (Blazor)

```razor
<MultiView ActiveViewIndex="@activeIndex">
    <View>
        <p>Step 1</p>
        <Button Text="Next" OnClick="() => activeIndex = 1" />
    </View>
    <View>
        <p>Step 2</p>
        <Button Text="Previous" OnClick="() => activeIndex = 0" />
    </View>
</MultiView>

@code {
    private int activeIndex = 0;
}
```

## See Also

- [MultiView](MultiView.md) — Parent container for View components
- [Panel](Panel.md) — Another container component
- [PlaceHolder](PlaceHolder.md) — Container with no wrapper element
