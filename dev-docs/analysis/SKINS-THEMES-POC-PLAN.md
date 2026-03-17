# Milestone 10 — Skins & Themes PoC Architecture Plan

**Author:** Forge (Lead / Web Forms Reviewer)
**Date:** 2026-02-25
**Milestone:** M10 — Sample Nav, Docs & Test Gaps (PoC portion)
**Full implementation:** Deferred to M11 — Skins and Themes Implementation

---

## 1. PoC Scope

### Question to Answer

> "Can we faithfully emulate `.skin` file behavior in Blazor, and what does the developer experience look like?"

### In Scope (M10 PoC)

- ✅ `ThemeProvider` component using `CascadingValue`
- ✅ `ThemeConfiguration` class with control-type defaults and named skins
- ✅ Base class integration — `BaseStyledComponent` reads from theme during init
- ✅ `SkinID` un-obsoleted and wired to named skin lookup
- ✅ `EnableTheming` un-obsoleted and wired to skip logic
- ✅ Demo with 2–3 controls (Button, Label, GridView) showing themed output
- ✅ Unit tests proving theme application, SkinID lookup, and EnableTheming opt-out

### Deferred to M11

- ❌ `.skin` file parser (reading actual `.skin` files)
- ❌ StyleSheetTheme vs Theme priority distinction
- ❌ CSS auto-bundling from theme folders
- ❌ Sub-component style theming (HeaderStyle, RowStyle, etc.)
- ❌ Container-level EnableTheming propagation to children
- ❌ Theme switching at runtime
- ❌ Build-time code generation from `.skin` files
- ❌ Migration tooling integration (M12 synergy)

---

## 2. Architecture Options

### Option A: CascadingValue ThemeProvider with C# Configuration

**How it works:**
- A `ThemeProvider` Blazor component wraps the app layout
- It holds a `ThemeConfiguration` object and provides it via `CascadingValue<ThemeConfiguration>`
- `ThemeConfiguration` contains `Dictionary<string, ControlSkin>` keyed by control type name
- Each `ControlSkin` has default properties and optional named skins
- `BaseWebFormsComponent.OnInitialized()` receives the cascading value and applies matching properties

```csharp
// Developer experience
<ThemeProvider Theme="@myTheme">
    <Router AppAssembly="...">
        ...
    </Router>
</ThemeProvider>

@code {
    ThemeConfiguration myTheme = new ThemeConfiguration()
        .ForControl("Button", skin => skin
            .Set(s => s.BackColor, WebColor.FromHtml("#FFDEAD"))
            .Set(s => s.Font.Bold, true))
        .ForControl("Button", "goButton", skin => skin
            .Set(s => s.BackColor, WebColor.FromHtml("#006633"))
            .Set(s => s.Width, new Unit("120px")));
}
```

| Pros | Cons |
|------|------|
| Type-safe, IDE-friendly | Requires converting `.skin` files to C# |
| No build tooling required | More verbose than `.skin` files |
| Testable with bUnit | Doesn't read existing `.skin` files |
| Idiomatic Blazor pattern | Learning curve for Web Forms developers used to `.skin` syntax |
| Works in all Blazor hosting models | |

### Option B: .skin File Parser (Read actual `.skin` files)

**How it works:**
- A parser reads `.skin` files from `wwwroot/themes/{ThemeName}/`
- Extracts control declarations and property values
- Builds a `ThemeConfiguration` at startup (or build time)
- Same `CascadingValue` delivery mechanism as Option A

```csharp
// Developer experience
builder.Services.AddWebFormsTheme("CoolTheme", "wwwroot/themes/CoolTheme/");
// or
<ThemeProvider ThemePath="themes/CoolTheme">
    ...
</ThemeProvider>
```

| Pros | Cons |
|------|------|
| Direct reuse of existing `.skin` files | Complex parser needed (pseudo-ASPX format) |
| Familiar to Web Forms developers | `.skin` format is not standard XML |
| Lower migration effort per theme | Runtime file reading may not work in all Blazor modes (WASM) |
| | Build-time vs runtime parsing decision |
| | Harder to test and debug |

### Option C: Blazor CSS Isolation + Component Parameters

**How it works:**
- Themes are pure CSS, leveraging Blazor's CSS isolation (`::deep` selectors)
- A `ThemeProvider` applies a CSS class to a wrapper `<div>`
- Component parameters are not set by themes — only CSS classes/styles change

```css
/* CoolTheme.razor.css */
::deep .webforms-button {
    background-color: #FFDEAD;
    font-weight: bold;
}
```

| Pros | Cons |
|------|------|
| Standard CSS — no custom infrastructure | Can't set non-CSS properties (Text, ToolTip, etc.) |
| Works with Blazor CSS isolation | Only visual properties — not behavioral |
| No special component logic needed | Doesn't match Web Forms skin semantics |
| | `.skin` files set component *properties*, not CSS rules |
| | Breaks the "same attributes and properties" principle |

### Option D: JSON Configuration Files

**How it works:**
- Themes defined in JSON files: `themes/CoolTheme/theme.json`
- Deserialized into `ThemeConfiguration` at startup
- Same `CascadingValue` delivery as Option A

```json
{
  "controls": {
    "Button": {
      "default": {
        "BackColor": "#FFDEAD",
        "Font-Bold": true,
        "BorderStyle": "Solid"
      },
      "goButton": {
        "Text": "Go",
        "Width": "120px",
        "BackColor": "#006633"
      }
    }
  }
}
```

| Pros | Cons |
|------|------|
| Easy to author and read | Not type-safe at authoring time |
| Closer to `.skin` file feel | Requires custom deserialization for WebColor, Unit, FontInfo |
| Can be loaded from static assets | No IDE IntelliSense without JSON schema |
| Works in all hosting models | Property name mapping complexity |

---

## 3. Recommended Approach

**Option A: CascadingValue ThemeProvider with C# Configuration**

### Rationale

1. **Type safety** — Property assignments are checked at compile time. A typo in `BackColor` fails the build, not at runtime. This is critical for a migration library where correctness is the #1 goal.

2. **IDE experience** — IntelliSense, refactoring, and Go to Definition all work. Developers migrating from Web Forms already face a learning curve; the tooling support reduces friction.

3. **Testability** — bUnit tests can create `ThemeConfiguration` objects directly, assert property values, and verify the full theming pipeline without file I/O or parsing.

4. **No build tooling** — Works immediately. No MSBuild targets, source generators, or file watchers needed. The PoC can ship as a pure library change.

5. **Incremental path** — Option B (`.skin` parser) can be layered on top later. The parser's output would be a `ThemeConfiguration` object, so Options A and B are complementary, not competing.

6. **Blazor-idiomatic** — CascadingValue is how Blazor apps share cross-cutting state (see: `CascadingAuthenticationState`, `EditContext`). Web Forms developers learning Blazor should learn this pattern.

Option C (CSS-only) is **rejected** because Web Forms skins set *component properties*, not CSS rules. A skin can set `Text="Go"` on a Button — CSS cannot do that.

Option D (JSON) is a viable future enhancement but adds deserialization complexity without compile-time safety. Better as a migration aid (M11/M12) than the core mechanism.

---

## 4. Work Items (PoC Sprint)

### WI-1: ThemeConfiguration data model
**Scope:** Create `ThemeConfiguration`, `ControlSkin`, and `SkinPropertyBag` classes in `src/BlazorWebFormsComponents/Theming/`.
- `ThemeConfiguration`: dictionary of control type → default `ControlSkin` + named skins
- `ControlSkin`: property bag with typed getters for style properties
- Fluent builder API: `.ForControl("Button", skin => skin.Set(...))`
- Unit tests for configuration building and lookup

**Effort:** S (Small)
**Assignee:** Cyclops

### WI-2: ThemeProvider cascading component
**Scope:** Create `ThemeProvider.razor` component that wraps content in `CascadingValue<ThemeConfiguration>`.
- Accepts `ThemeConfiguration Theme` parameter
- Renders `@ChildContent` inside cascading value
- Supports nesting (inner ThemeProvider overrides outer)

**Effort:** S (Small)
**Assignee:** Cyclops

### WI-3: Base class theme integration
**Scope:** Modify `BaseWebFormsComponent` and `BaseStyledComponent` to read and apply theme properties.
- Add `[CascadingParameter] ThemeConfiguration Theme` to `BaseWebFormsComponent`
- In `OnInitialized`, resolve control skin by type name and `SkinID`
- Apply skin properties to component parameters (only if not explicitly set)
- Respect `EnableTheming=false` to skip
- Remove `[Obsolete]` from `SkinID` and `EnableTheming`

**Effort:** M (Medium)
**Assignee:** Cyclops

### WI-4: PoC demo and sample page
**Scope:** Create a sample page demonstrating theming with Button, Label, and GridView.
- Define a theme configuration in code
- Show default skins applied automatically
- Show named skins via SkinID
- Show EnableTheming=false opt-out
- Before/after comparison with Web Forms equivalent

**Effort:** S (Small)
**Assignee:** Jubilee

### WI-5: bUnit tests for theming pipeline
**Scope:** Write tests proving the theming pipeline works end-to-end.
- Test: default skin applies to Button
- Test: named skin applies when SkinID matches
- Test: EnableTheming=false prevents skin application
- Test: explicit parameter values are not overridden by theme (StyleSheetTheme behavior)
- Test: missing SkinID is handled gracefully (no throw, log warning)

**Effort:** S (Small)
**Assignee:** Rogue

---

## 5. Success Criteria

The PoC is viable if **all** of the following are true:

| # | Criterion | How to Verify |
|---|-----------|---------------|
| 1 | A `ThemeConfiguration` can express default and named skins for multiple control types | Unit test: build config, query properties, assert values |
| 2 | `ThemeProvider` delivers configuration to nested components via `CascadingValue` | bUnit test: render Button inside ThemeProvider, assert themed output |
| 3 | Default skins apply automatically without SkinID | bUnit test: Button with no SkinID gets theme's BackColor |
| 4 | Named skins apply when SkinID matches | bUnit test: Button with SkinID="goButton" gets named skin's Width |
| 5 | `EnableTheming=false` prevents theme application | bUnit test: Button with EnableTheming=false renders without theme |
| 6 | Explicitly set parameters are not overridden | bUnit test: `<Button BackColor="Red">` inside theme still shows Red |
| 7 | Sample page visually demonstrates themed controls | Manual review: page renders controls with theme styling |
| 8 | All existing tests continue to pass | CI green: no regressions from theme infrastructure |

---

## 6. Open Questions for Jeff

1. **Theme mode default:** Should the default behavior be `StyleSheetTheme` (theme as defaults, page overrides) or `Theme` (theme overrides page)? Web Forms apps use `Theme` more commonly, but `StyleSheetTheme` is safer for migration. **Recommendation:** Default to `StyleSheetTheme` semantics (don't override explicit values).

2. **Namespace/folder:** Is `BlazorWebFormsComponents.Theming` acceptable for the new types, or should they go in the root namespace?

3. **`.skin` parser priority:** Should M11 focus on a `.skin` file parser (to read existing files directly), or is C#/JSON configuration sufficient? This affects whether Web Forms developers can drag-and-drop their `.skin` files.

4. **Error handling:** When a component specifies `SkinID="foo"` but no skin named "foo" exists, should we: (a) throw an exception (Web Forms behavior), (b) log a warning and continue, or (c) silently ignore? **Recommendation:** (b) log a warning.

5. **Sub-component styles scope:** GridView/DetailsView have 8–12 style sub-components each (HeaderStyle, RowStyle, etc.). Should M11 theme these individually, or only theme the top-level control? **Recommendation:** Support sub-component styles — they're the most common use of GridView skins.

---

## 7. GitHub Issues

The following issues have been created for this PoC:

| WI | Issue # | Title | Milestone |
|----|---------|-------|-----------|
| WI-1 | [#364](https://github.com/FritzAndFriends/BlazorWebFormsComponents/issues/364) | ThemeConfiguration data model | M10 |
| WI-2 | [#365](https://github.com/FritzAndFriends/BlazorWebFormsComponents/issues/365) | ThemeProvider cascading component | M10 |
| WI-3 | [#366](https://github.com/FritzAndFriends/BlazorWebFormsComponents/issues/366) | Base class theme integration | M10 |
| WI-4 | [#367](https://github.com/FritzAndFriends/BlazorWebFormsComponents/issues/367) | PoC demo and sample page for theming | M10 |
| WI-5 | [#368](https://github.com/FritzAndFriends/BlazorWebFormsComponents/issues/368) | bUnit tests for theming pipeline | M10 |
| M11 | [#369](https://github.com/FritzAndFriends/BlazorWebFormsComponents/issues/369) | M11 Planning: Full Skins & Themes implementation | M11 |

*(Issue numbers will be updated after creation)*

---

## Appendix: M11 Scope Preview

Based on this PoC, M11 should cover:

- StyleSheetTheme vs Theme priority mode
- `.skin` file parser (for direct migration of existing files)
- Sub-component style theming (HeaderStyle, RowStyle, etc.)
- CSS file bundling from theme folders
- Container-level EnableTheming propagation
- Runtime theme switching
- JSON theme format as alternative input
- Migration tooling integration (`.skin` → C# converter in M12 tool)
- Documentation: migration guide for Web Forms themes
