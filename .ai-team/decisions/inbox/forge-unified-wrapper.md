# Design: Unified Legacy Web Forms Support Wrapper

**Author:** Forge (Lead / Web Forms Reviewer)
**Date:** 2026-02-26
**Status:** Proposed

---

## Recommended Name: `WebFormsPage`

In ASP.NET Web Forms, `System.Web.UI.Page` was the root of every `.aspx` page. It established the naming container root (`ctl00`), applied the theme from `<%@ Page Theme="..." %>`, and held the ViewState dictionary. `WebFormsPage` mirrors that concept directly. Developers who lived in Web Forms will immediately understand what this component does: "this is my Page."

Rejected alternatives:
- `WebFormsSupport` — too generic, sounds like a utility, not a structural root
- `WebFormsHost` — implies hosting infrastructure, not page structure
- `LegacyPage` — pejorative; we want devs to use this without feeling bad about it

---

## Parameters

| Parameter | Type | Default | Source |
|---|---|---|---|
| `ID` | `string` | `null` | Inherited from `BaseWebFormsComponent` |
| `UseCtl00Prefix` | `bool` | `false` | From `NamingContainer` |
| `Theme` | `ThemeConfiguration` | `null` | From `ThemeProvider` |
| `Visible` | `bool` | `true` | Inherited from `BaseWebFormsComponent` |
| `ChildContent` | `RenderFragment` | — | Standard |

No new parameters invented. This is strictly composition of existing behaviors.

---

## Implementation Strategy: Inherit NamingContainer, Compose ThemeProvider

`WebFormsPage` should **inherit from `NamingContainer`** (not `BaseWebFormsComponent` directly). This gives it:
- The `UseCtl00Prefix` parameter
- The `ID` / `ClientID` / `Parent` cascading from `BaseWebFormsComponent`
- The `Visible` gate

It then **adds** the `Theme` parameter and wraps `ChildContent` in a `CascadingValue<ThemeConfiguration>`, composing ThemeProvider's behavior inline.

### Razor markup

```razor
@inherits NamingContainer

@if (Visible)
{
    <CascadingValue Value="Theme">
        @ChildContent
    </CascadingValue>
}
```

### Code-behind

```csharp
namespace BlazorWebFormsComponents;

public partial class WebFormsPage : NamingContainer
{
    [Parameter]
    public ThemeConfiguration Theme { get; set; }
}
```

Key points:
- When `Theme` is `null`, the `CascadingValue` still cascades `null` — this is fine. Child components that check `[CascadingParameter] ThemeConfiguration` will get `null` and skip theming. This matches existing ThemeProvider behavior (no null guard needed).
- `ChildContent` is already declared on `NamingContainer`. Do NOT redeclare it in `WebFormsPage` or you'll get a compile error. The Razor `@ChildContent` reference uses the inherited property.

### Nesting order

The cascading values nest as:

```
CascadingValue<BaseWebFormsComponent> (Name="ParentComponent")  ← from BaseWebFormsComponent ctor
  └─ CascadingValue<ThemeConfiguration>                          ← from WebFormsPage.razor
       └─ ChildContent                                           ← developer's page content
```

This matches Web Forms semantics: Page is the root naming container AND the theme applicator.

---

## Placement: Layout, Not Router

`WebFormsPage` belongs in **`MainLayout.razor`**, wrapping `@Body`:

```razor
@inherits LayoutComponentBase

<WebFormsPage ID="MainContent" UseCtl00Prefix="true" Theme="@_theme">
    @Body
</WebFormsPage>

@code {
    private ThemeConfiguration _theme = new();
}
```

**Why layout, not router:**
- `Routes.razor` owns routing mechanics (`<Router>`, `<RouteView>`). Wrapping `<Router>` adds a naming container above the layout, which is wrong — in Web Forms, `Page` was the layout, not the router.
- The layout runs per-page and wraps `@Body`, which is the actual page content. This is the exact analog of the `<form runat="server">` tag that wrapped page content in Web Forms.
- Placing it in the layout means every page automatically gets naming + theming without any per-page markup.

**Alternative per-page usage** (for apps that need different themes per area):

```razor
@page "/admin"

<WebFormsPage ID="AdminContent" Theme="@_adminTheme">
    <!-- admin content here -->
</WebFormsPage>
```

This is also valid. The component doesn't mandate layout placement.

---

## ViewState: Do NOT Include

ViewState is a `Dictionary<string, object>` property already baked into `BaseWebFormsComponent` (line 127). Every component — including `WebFormsPage` via inheritance — already has it. There's nothing to cascade or compose.

In Web Forms, `Page.ViewState` was just the page-level instance of the same dictionary every control had. The serialization-to-hidden-field behavior is intentionally not replicated (it was the #1 performance problem in Web Forms). The in-memory dictionary is already there.

**Verdict:** ViewState is already handled. No action needed.

---

## Backward Compatibility

| Concern | Assessment |
|---|---|
| Existing `NamingContainer` users | **No break.** `NamingContainer` remains a standalone component. `WebFormsPage` inherits it, so standalone usage is unaffected. |
| Existing `ThemeProvider` users | **No break.** `ThemeProvider.razor` stays where it is. `WebFormsPage` composes the same cascading pattern but doesn't touch `ThemeProvider`. |
| Namespace | `WebFormsPage` should live in the root namespace `BlazorWebFormsComponents` (same as `NamingContainer`), not in `Theming`. It's a page-level structural component, not a theming utility. |
| Samples | The theming sample at `samples/AfterBlazorServerSide/Components/Pages/ControlSamples/Theming/Index.razor` should continue to use standalone `ThemeProvider` — it demonstrates theming in isolation. |

---

## File Locations

- `src/BlazorWebFormsComponents/WebFormsPage.razor` — the Razor markup
- `src/BlazorWebFormsComponents/WebFormsPage.razor.cs` — the code-behind with `Theme` parameter
- Tests: `src/BlazorWebFormsComponents.Test/WebFormsPage/WebFormsPageTests.razor`

---

## Open Question for Jeff

Should `WebFormsPage` set a **default `ID`** (e.g., `"Page"`) when none is provided? In Web Forms, the Page always had an implicit naming container even without an explicit ID. Setting a default would mean child components always get a prefixed ClientID. Leaving it null means the naming container is transparent (no prefix) unless the developer sets `ID` explicitly. I lean toward **no default** — let the developer opt in — but Jeff should decide.

---

## Summary for Cyclops

Build `WebFormsPage` as:
1. Two files: `.razor` + `.razor.cs`
2. Inherits `NamingContainer`
3. Adds one parameter: `Theme` (`ThemeConfiguration`)
4. Razor: `@if (Visible) { <CascadingValue Value="Theme"> @ChildContent </CascadingValue> }`
5. Root namespace: `BlazorWebFormsComponents`
6. Do NOT redeclare `ChildContent` in code-behind (inherited from `NamingContainer`)
7. Write tests verifying: naming cascading works through WebFormsPage, theme cascading works through WebFormsPage, both work together
