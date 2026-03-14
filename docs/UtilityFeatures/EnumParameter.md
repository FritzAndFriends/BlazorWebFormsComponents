# EnumParameter\<T\>

`EnumParameter<T>` is a transparent wrapper struct that allows Blazor component parameters to accept **both** enum values and plain strings. This eliminates the most common migration friction point: the Razor compiler requiring `@()` expression syntax for enum values.

Original Web Forms controls accepted string attribute values and parsed them at runtime. Blazor's strongly-typed parameter binding normally requires explicit enum syntax. `EnumParameter<T>` bridges this gap.

## Background

In ASP.NET Web Forms, you could write:

```html
<asp:GridView GridLines="None" runat="server" />
<asp:Panel ScrollBars="Vertical" runat="server" />
```

Web Forms parsed the string `"None"` into the `GridLines` enum at runtime. In Blazor, enum parameters normally require Razor expression syntax:

```razor
@* Without EnumParameter — requires wrapping in @() *@
<GridView GridLines="@(GridLines.None)" />
<Panel ScrollBars="@(ScrollBars.Vertical)" />
```

This means every migrated page needs manual touch-up of enum attributes, which is error-prone at scale.

## How It Works

With `EnumParameter<T>`, both syntaxes work:

```razor
@* String value — just like Web Forms *@
<GridView GridLines="None" />
<Panel ScrollBars="Vertical" />

@* Enum value — standard Blazor syntax still works *@
<GridView GridLines="@(GridLines.None)" />
```

The wrapper provides implicit conversions in both directions:

- **`string` → `EnumParameter<T>`**: Parses the string (case-insensitive) into the enum value
- **`T` → `EnumParameter<T>`**: Wraps the enum value directly
- **`EnumParameter<T>` → `T`**: Unwraps back to the original enum transparently

## Supported Components

`EnumParameter<T>` is used across **46 components** in the library, covering all enum parameters:

| Component | Parameters |
|---|---|
| GridView | `GridLines`, `SortDirection`, `HorizontalAlign` |
| DetailsView | `GridLines`, `HorizontalAlign` |
| Panel | `ScrollBars`, `HorizontalAlign`, `Direction` |
| TextBox | `TextMode` |
| Calendar | `DayNameFormat`, `FirstDayOfWeek`, `NextPrevFormat`, `TitleFormat` |
| BulletedList | `BulletStyle`, `DisplayMode` |
| UpdatePanel | `UpdateMode`, `RenderMode` |
| ScriptManager | `ScriptMode` |
| Login | `Orientation` |
| Localize | `Mode` |
| *...and many more* | |

## Usage Notes

### In Migrated Markup

No changes needed — string enum values from Web Forms work as-is:

```razor
<GridView DataSource="@products"
          GridLines="Both"
          HorizontalAlign="Center"
          AutoGenerateColumns="true" />
```

### In Code-Behind

When comparing `EnumParameter<T>` values in C# code, the implicit conversion handles most cases:

```csharp
@code {
    // Direct comparison works via implicit conversion
    private EnumParameter<GridLines> _gridLines = GridLines.None;

    private void CheckGridLines()
    {
        if (_gridLines == GridLines.None) { /* works */ }
    }
}
```

!!! warning "Switch Statements"
    C# pattern matching does not use implicit conversions. In `switch` statements, use `.Value`:

    ```csharp
    switch (gridLines.Value)  // Not: switch (gridLines)
    {
        case GridLines.None: break;
        case GridLines.Both: break;
    }
    ```

!!! warning "Shouldly / Test Assertions"
    Some assertion libraries don't resolve extension methods for generic structs. Use `.Value` in test assertions:

    ```csharp
    cut.Instance.GridLines.Value.ShouldBe(GridLines.None);  // ✓
    // cut.Instance.GridLines.ShouldBe(GridLines.None);      // ✗ won't compile
    ```

### Invalid Values

If an invalid string is passed, `EnumParameter<T>` throws an `ArgumentException` at parameter binding time:

```razor
@* This will throw ArgumentException — "Invalid" is not a GridLines value *@
<GridView GridLines="Invalid" />
```

## Moving On

`EnumParameter<T>` is a migration convenience. As you modernize your Blazor application:

1. **Keep string syntax** if it improves readability — there's no performance penalty
2. **Switch to enum syntax** (`@(GridLines.None)`) when you want compile-time safety and IDE IntelliSense
3. The wrapper has zero runtime overhead for enum-value assignments; string parsing only occurs for string-value assignments

## See Also

- [GridView](../DataControls/GridView.md) — Example of a component using `EnumParameter<T>`
- [WebFormsPage](WebFormsPage.md) — Page-level migration base class
- [L2 Automation Shims](L2AutomationShims.md) — Overview of all migration automation features
