# ViewState

One of the most abused and misunderstood features of ASP<span></span>.NET Web Forms is ViewState.  Originally designed to help stateful Visual Basic 6 Windows application developers migrate to the web, this feature delivered a lot of heartache to developers who didn't fully understand and limit its usage appropriately in their web applications.  In Web Forms, every `Page` and every `Control` carries a ViewState object that contains the state of the Control or Page at the time of request, serialized and output as an HTML hidden input tag.  Without proper diligence in selecting which controls and features need their state tracked, many applications found this HTML tag fill up with megabytes of serialized code and slowing their applications.

Elsewhere in Web Forms, ViewState could be used as a key/value store to place data that should be remembered during the current fetch/post/retrieve cycle for a page.  You would find syntax like the following in Pages and Controls to add the `bar` object with a key named `foo`:

```c#
ViewState.Add("foo", bar);
```

or to fetch the item named `foo` from ViewState:

```c#
ViewState["foo"]
```

It's a simple syntax and implements a typical Dictionary pattern.  After the PreRender phase of a Page, the content would be serialized and inserted into an HTML hidden input so that it would be available when the Page was posted.

## Implementation in BlazorWebFormsComponents

BlazorWebFormsComponents now provides `ViewStateDictionary`, a comprehensive implementation of ViewState that works seamlessly across rendering modes:

- **SSR (Server-Side Rendering)** — ViewState automatically serializes to a protected hidden form field for round-tripping through HTTP POSTs
- **ServerInteractive (Blazor WebSocket)** — ViewState persists in component memory for the lifetime of the component instance

This allows Web Forms patterns to migrate with minimal markup changes.

### Quick Start

```csharp
// Store a value
ViewState["UserPreferences"] = new UserSettings { Theme = "Dark" };

// Retrieve a value (null-safe)
var prefs = ViewState["UserPreferences"];

// Type-safe convenience method
ViewState.Set<UserSettings>("UserPreferences", userSettings);
UserSettings retrieved = ViewState.GetValueOrDefault<UserSettings>("UserPreferences", new());
```

### Detecting Postbacks

Use the `IsPostBack` property to distinguish between first render and postback:

```csharp
protected override void OnInitialized()
{
    if (!IsPostBack)
    {
        // First render: load initial data
        LoadProducts();
    }
    else
    {
        // Postback: restore state from ViewState
        RestoreState();
    }
}
```

### Hidden Field Persistence (SSR Only)

When running in SSR, ViewState is automatically round-tripped through a protected hidden form field. Manually emit the field when needed:

```razor
<form method="post">
    <TextBox ID="Name" @bind-Value="_name" />
    <Button Text="Save" @onclick="OnSave" />
    
    <RenderViewStateField />
</form>
```

## Moving On

ViewState is not a feature you should continue to use after migrating.  While we have already eliminated the serializeation / deserialization performance issue with ViewState, we still recommend moving any storage you do with ViewState to class-scoped fields and properties that can be validated and strongly-typed.  This will also reduce the boxing/unboxing performance issue that exists with ViewState.

You would define the property for Foo with a syntax similar to, substituting the proper type of bar for `BarType`:

```c#
protected BarType Foo { get; set; }
```

The interaction above would be updated to the following syntax in your component's class:

```c#
// Store bar for retrieval later
Foo = bar;

// Fetch bar for use
bar = Foo;
```

## See Also

- [ViewState and PostBack Shim](ViewStateAndPostBack.md) — Comprehensive guide covering ViewStateDictionary, mode-adaptive IsPostBack, hidden field persistence, and migration patterns