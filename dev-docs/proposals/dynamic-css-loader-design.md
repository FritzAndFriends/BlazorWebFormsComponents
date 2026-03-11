# Dynamic CSS Loader Design Proposal

**Author:** Cyclops (Component Dev)  
**Date:** 2026-03-10  
**Status:** Draft

## Executive Summary

This document analyzes the HeadContent limitation in Blazor layouts and proposes a `PageStyleSheet` component for dynamic CSS loading/unloading during Web Forms migrations.

---

## Part 1: Why HeadContent Doesn't Work in Layouts

### The Problem

When migrating Web Forms applications, CSS from `Site.Master` would naturally be placed in the Blazor `MainLayout.razor`. However, placing `<HeadContent>` in a layout does NOT inject content into `<HeadOutlet>`:

```razor
@* MainLayout.razor - THIS DOES NOT WORK *@
@inherits LayoutComponentBase

<HeadContent>
    <link href="/CSS/Master_CSS.css" rel="stylesheet" />
</HeadContent>

@Body
```

### Technical Explanation

Blazor's HeadOutlet/HeadContent system has a **"last writer wins"** architecture, not an aggregation model:

1. **Render Order:** Blazor renders from top (`App.razor`) → Layout → Page. The `HeadOutlet` collects content from `HeadContent` components.

2. **Overwrite Behavior:** When multiple `HeadContent` components exist in the tree, only the most downstream one (typically the page) is rendered. Layout content is **replaced**, not merged.

3. **Design Rationale:** This was intentional for `<PageTitle>` (one title per page makes sense), but the same architecture was applied to `HeadContent` for simplicity.

4. **GitHub Issues:** This is a known limitation:
   - [#45904](https://github.com/dotnet/aspnetcore/issues/45904) - "HeadContent don't merge in LayoutPage and Index Page"
   - [#51864](https://github.com/dotnet/aspnetcore/issues/51864) - "HeadContent in .razor page should be APPENDED to"

### Can We Create a Custom HeadOutlet?

**Short answer: Not without significant complexity.**

The `HeadOutlet` component is tightly integrated with Blazor's rendering pipeline:
- It uses internal Blazor types (`HeadContentService`, cascade values)
- The "last one wins" behavior is baked into `Microsoft.AspNetCore.Components.Web.HeadContent`
- Creating a custom aggregating outlet would require reimplementing head content tracking

**Alternative: SectionContent/SectionOutlet (.NET 8+)**

.NET 8 introduced `SectionContent` and `SectionOutlet` which CAN aggregate:

```razor
@* In App.razor - wrap HeadOutlet with a section *@
<head>
    <HeadOutlet />
    <SectionOutlet SectionName="AdditionalHead" />
</head>

@* In layouts or pages *@
<SectionContent SectionName="AdditionalHead">
    <link href="/CSS/Master_CSS.css" rel="stylesheet" />
</SectionContent>
```

However, this requires:
1. Modifying `App.razor` (not just the layout)
2. Multiple pages/layouts adding to the same section
3. Understanding Blazor's section system (steeper learning curve for migrators)

---

## Part 2: PageStyleSheet Component Design

### Requirements

1. **Dynamic Loading:** Load CSS when component renders
2. **Automatic Cleanup:** Remove CSS when component disposes (page navigation)
3. **Web Forms Familiarity:** Match the pattern from page-specific CSS:
   ```aspx
   <asp:Content ContentPlaceHolderID="head">
       <link href="CSS/CSS_Courses.css" rel="stylesheet" />
   </asp:Content>
   ```
4. **SSR Compatible:** Work with Blazor's streaming/SSR rendering modes

### Proposed Component API

```razor
@* Basic usage *@
<PageStyleSheet Href="CSS/CSS_Courses.css" />

@* Multiple stylesheets *@
<PageStyleSheet Href="CSS/CSS_Courses.css" />
<PageStyleSheet Href="CSS/Vendors/DataGrid.css" />

@* With optional attributes *@
<PageStyleSheet Href="CSS/CSS_Courses.css" Media="screen" />
<PageStyleSheet Href="CSS/Print.css" Media="print" />
```

### Implementation Strategy

#### Option A: JavaScript Interop (Recommended)

Use JS interop to dynamically inject `<link>` elements:

**Pros:**
- Works in all render modes (SSR, InteractiveServer, InteractiveWebAssembly)
- Proper cleanup on dispose
- No modifications to App.razor required

**Cons:**
- Requires JavaScript (minimal)
- Flash of unstyled content (FOUC) possible on first load

**Implementation:**

```csharp
// PageStyleSheet.razor.cs
public partial class PageStyleSheet : ComponentBase, IAsyncDisposable
{
    [Inject] private IJSRuntime JS { get; set; } = null!;
    
    [Parameter, EditorRequired] public string Href { get; set; } = "";
    [Parameter] public string? Media { get; set; }
    [Parameter] public string? Id { get; set; }
    
    private string _linkId = "";
    private bool _isLoaded;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !string.IsNullOrEmpty(Href))
        {
            _linkId = Id ?? $"bwfc-css-{Guid.NewGuid():N}";
            await LoadStyleSheetAsync();
            _isLoaded = true;
        }
    }
    
    private async Task LoadStyleSheetAsync()
    {
        await JS.InvokeVoidAsync("bwfc.loadStyleSheet", _linkId, Href, Media);
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_isLoaded)
        {
            try
            {
                await JS.InvokeVoidAsync("bwfc.unloadStyleSheet", _linkId);
            }
            catch (JSDisconnectedException)
            {
                // Circuit disconnected - stylesheet will be cleaned up naturally
            }
        }
    }
}
```

**JavaScript:**

```javascript
// In Basepage.module.js
export function loadStyleSheet(id, href, media) {
    // Check if already loaded (idempotent)
    if (document.getElementById(id)) return;
    
    const link = document.createElement('link');
    link.id = id;
    link.rel = 'stylesheet';
    link.href = href;
    if (media) link.media = media;
    
    document.head.appendChild(link);
}

export function unloadStyleSheet(id) {
    const link = document.getElementById(id);
    if (link) {
        link.remove();
    }
}
```

#### Option B: HeadContent with Static Rendering

For SSR-only scenarios, use HeadContent directly (but accept the layout limitation):

```razor
@* PageStyleSheet.razor *@
<HeadContent>
    <link href="@Href" rel="stylesheet" media="@Media" />
</HeadContent>
```

**Pros:**
- No JavaScript
- No FOUC

**Cons:**
- Only works in page components (not layouts)
- No dispose cleanup (CSS stays in head until full page reload)
- Conflicts with page's own HeadContent

### Recommendation: Hybrid Approach

Combine both approaches based on render mode:

```csharp
public partial class PageStyleSheet : ComponentBase, IAsyncDisposable
{
    [Inject] private IJSRuntime JS { get; set; } = null!;
    
    [Parameter, EditorRequired] public string Href { get; set; } = "";
    [Parameter] public string? Media { get; set; }
    [Parameter] public string? Id { get; set; }
    
    private string _linkId = "";
    private bool _jsEnabled;
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !string.IsNullOrEmpty(Href))
        {
            _linkId = Id ?? $"bwfc-css-{Guid.NewGuid():N}";
            try
            {
                await JS.InvokeVoidAsync("bwfc.loadStyleSheet", _linkId, Href, Media);
                _jsEnabled = true;
            }
            catch (InvalidOperationException)
            {
                // SSR-only mode, JS not available - fall through
            }
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_jsEnabled)
        {
            try
            {
                await JS.InvokeVoidAsync("bwfc.unloadStyleSheet", _linkId);
            }
            catch { /* Circuit gone */ }
        }
    }
}
```

---

## Part 3: Extended DynamicHead Component

For full Web Forms ContentPlaceHolder="head" migration, we could provide a more general component:

```razor
@* DynamicHead.razor - for arbitrary head content *@
<DynamicHead>
    <link href="CSS/CSS_Courses.css" rel="stylesheet" />
    <script src="JQuery/JQuery_Courses.js"></script>
    <meta name="page-specific" content="courses" />
</DynamicHead>
```

However, this is significantly more complex:
- Must serialize child content to HTML string
- JS must parse and inject arbitrary elements
- Script injection has security implications
- Not recommended for initial implementation

**Recommendation:** Start with `PageStyleSheet` for CSS only. Add script support separately if needed.

---

## Part 4: Interaction with Render Modes

| Render Mode | PageStyleSheet Behavior |
|-------------|------------------------|
| **Static SSR** | JS loads CSS on enhanced navigation; no automatic unload |
| **InteractiveServer** | JS loads CSS; proper dispose cleanup |
| **InteractiveWebAssembly** | JS loads CSS; proper dispose cleanup |
| **Auto** | Works in both modes |

### SSR Considerations

In Static SSR with enhanced navigation:
- `OnAfterRenderAsync` fires after hydration
- CSS loads dynamically
- Disposal doesn't fire on navigation (page just updates)

**Solution:** For SSR, consider a service that tracks loaded stylesheets and cleans up on navigation:

```csharp
public interface IStyleSheetTracker
{
    void Track(string href);
    Task CleanupOnNavigationAsync();
}
```

---

## Part 5: Implementation Plan

### Phase 1: Core Component (Recommended Starting Point)

1. Add JS functions to `Basepage.module.js`
2. Create `PageStyleSheet.razor` and `PageStyleSheet.razor.cs`
3. Add to `_Imports.razor`
4. Test in AfterContosoUniversity sample

### Phase 2: Enhanced Features

1. Add `IStyleSheetTracker` for SSR navigation cleanup
2. Add preload support: `<PageStyleSheet Href="..." Preload="true" />`
3. Add integrity/crossorigin attributes for CDN usage

### Phase 3: Script Support (Future)

1. Create `PageScript` component for dynamic script loading
2. Handle script dependencies and load ordering

---

## Appendix: Migration Pattern for Courses.aspx

**Original Web Forms:**
```aspx
<asp:Content ContentPlaceHolderID="head" runat="server">
    <link href="CSS/CSS_Courses.css" rel="stylesheet" />
    <script src="JQuery/JQuery_Courses.js"></script>
</asp:Content>
```

**Migrated Blazor (with PageStyleSheet):**
```razor
@page "/Courses"

<PageStyleSheet Href="CSS/CSS_Courses.css" />
@* Script loading handled separately *@

<div id="dropList">
    ...
</div>
```

---

## Decision Required

- [ ] Approve Phase 1 implementation of `PageStyleSheet`
- [ ] Decide on SSR navigation cleanup approach
- [ ] Determine if script loading should be in scope
