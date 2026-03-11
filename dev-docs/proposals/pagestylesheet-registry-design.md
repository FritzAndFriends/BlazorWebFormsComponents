# PageStyleSheet Registry Architecture

**Author:** Forge (Lead / Web Forms Reviewer)  
**Date:** 2026-03-11  
**Status:** Proposal  
**Reviewers:** Jeffrey T. Fritz, Cyclops

---

## Executive Summary

The current PageStyleSheet implementation has a fundamental lifecycle mismatch: CSS unloads on `DisposeAsync`, but in SSR mode dispose fires immediately after render—before the user sees the page. The current fix (static `<link>` + smart disposal) works for SSR, but doesn't solve the broader problem of CSS lifecycle management in Blazor's enhanced navigation model.

**This proposal introduces a registry-based "last page wins" model** where CSS persists until no PageStyleSheet component in the render tree references it anymore.

---

## Part 1: Problem Analysis

### Current Behavior (After Cyclops Fix)

| Mode | Load | Unload |
|------|------|--------|
| Static SSR | Static `<link>` | Browser (full page nav) |
| Prerender → Interactive | Static `<link>` | JS on dispose |
| InteractiveServer | JS interop | JS on dispose |

### The Problem

In Blazor's enhanced navigation model:
1. **Layout CSS** should persist across page navigations (layout stays alive)
2. **Page CSS** should swap when pages swap
3. **Current dispose-based unload** fires at the wrong time for SSR/prerender

### The Insight

**The CSS lifecycle should be tied to the component tree, not dispose timing.**

- If a PageStyleSheet component is **in the render tree** → its CSS stays loaded
- If a PageStyleSheet component is **removed from the render tree** → its CSS can be cleaned up

This naturally handles:
- **Layout CSS** persists because the layout component stays alive across navigations
- **Page CSS** swaps because the old page leaves the tree and the new one enters

---

## Part 2: Registry Architecture

### Design Goals

1. **Global stylesheet registry** tracks all active PageStyleSheet instances
2. **Reference counting** handles multiple components referencing the same CSS
3. **Cleanup after navigation settles** — not on individual dispose
4. **SSR compatibility** — works with static `<link>` tags that JS takes over later

### Where Should the Registry Live?

#### Option A: C# Scoped Service

```csharp
public class StyleSheetRegistry
{
    private readonly Dictionary<string, HashSet<string>> _activeSheets = new();
    // Key = Href, Value = Set of component IDs referencing it
    
    public void Register(string componentId, string href) { ... }
    public void Unregister(string componentId) { ... }
    public IEnumerable<string> GetOrphanedHrefs() { ... }
}
```

**Pros:**
- Type-safe, testable
- Full lifecycle control
- Can integrate with NavigationManager

**Cons:**
- Scoped to circuit (InteractiveServer) or app (WASM)
- In SSR, each request gets a new service—no persistence
- Must coordinate with JS for actual DOM manipulation

#### Option B: JavaScript Global State

```javascript
// In Basepage.module.js
const stylesheetRegistry = {
    refs: new Map(), // href -> Set<componentId>
    links: new Map(), // href -> link element
    
    register(componentId, href) { ... },
    unregister(componentId, href) { ... },
    cleanupOrphans() { ... }
};
```

**Pros:**
- Persists across enhanced navigations (same document)
- Direct DOM access
- Can use MutationObserver for automatic cleanup
- Works naturally with SSR static `<link>` tags

**Cons:**
- No C# compile-time safety
- Harder to unit test
- Must handle page unload edge cases

#### **Recommendation: Hybrid (JS Primary, C# Coordination)**

The **registry should live in JavaScript** because:
1. CSS manipulation is DOM work—JS is the natural home
2. JS state persists across enhanced navigations (SSR + interactive)
3. Static `<link>` tags from SSR can be "adopted" by the JS registry
4. MutationObserver can detect when components leave the DOM

C# components **register/unregister via JS interop** but don't hold the canonical state.

---

## Part 3: Lifecycle Flow

### 1. Initial Page Load (Static SSR)

```
┌─────────────────────────────────────────────────────────────────┐
│  Server Render                                                  │
├─────────────────────────────────────────────────────────────────┤
│  1. PageStyleSheet.razor renders static <link> tag             │
│     <link id="bwfc-css-{guid}" href="CSS/Page.css" />          │
│                                                                 │
│  2. HTML sent to browser (CSS visible immediately)             │
│                                                                 │
│  3. Component disposes on server (no JS interop → no-op)       │
└─────────────────────────────────────────────────────────────────┘

┌─────────────────────────────────────────────────────────────────┐
│  Browser (hydration/enhancement)                                │
├─────────────────────────────────────────────────────────────────┤
│  4. blazor.web.js enhances the page                            │
│                                                                 │
│  5. Blazor re-renders components (same static <link> output)   │
│                                                                 │
│  6. OnAfterRenderAsync runs → JS registry.register()           │
│     - Finds existing <link> by ID → adopts it                  │
│     - Records: refs["CSS/Page.css"] = Set(["bwfc-css-{guid}"]) │
└─────────────────────────────────────────────────────────────────┘
```

### 2. Enhanced Navigation (Page Swap)

```
┌─────────────────────────────────────────────────────────────────┐
│  User clicks link (enhanced navigation)                        │
├─────────────────────────────────────────────────────────────────┤
│  1. Old page's PageStyleSheet disposes:                        │
│     await JS.InvokeVoidAsync("registry.unregister", id, href)  │
│     - refs["CSS/OldPage.css"] = Set() (empty)                  │
│                                                                 │
│  2. New page's PageStyleSheet renders:                         │
│     await JS.InvokeVoidAsync("registry.register", id, href)    │
│     - refs["CSS/NewPage.css"] = Set(["bwfc-css-newguid"])      │
│                                                                 │
│  3. After navigation settles (see Part 4):                     │
│     registry.cleanupOrphans()                                  │
│     - Removes CSS/OldPage.css (ref count = 0)                  │
└─────────────────────────────────────────────────────────────────┘
```

### 3. Layout CSS Persistence

```
┌─────────────────────────────────────────────────────────────────┐
│  Layout.razor (persists across navigations)                    │
├─────────────────────────────────────────────────────────────────┤
│  <PageStyleSheet Href="CSS/Layout.css" Id="layout-css" />      │
│                                                                 │
│  - Registered once on first page load                          │
│  - Never disposed (layout stays alive)                         │
│  - refs["CSS/Layout.css"] always has "layout-css"              │
│  - Never orphaned → never unloaded                             │
└─────────────────────────────────────────────────────────────────┘
```

### 4. Multiple Components, Same CSS

```
┌─────────────────────────────────────────────────────────────────┐
│  Page A: <PageStyleSheet Href="shared.css" Id="shared-a" />    │
│  Page B: <PageStyleSheet Href="shared.css" Id="shared-b" />    │
├─────────────────────────────────────────────────────────────────┤
│  If both pages are in tree:                                    │
│    refs["shared.css"] = Set(["shared-a", "shared-b"])          │
│                                                                 │
│  Page A disposes:                                               │
│    refs["shared.css"] = Set(["shared-b"]) (still has ref)      │
│    → CSS NOT removed                                           │
│                                                                 │
│  Page B disposes:                                               │
│    refs["shared.css"] = Set() (empty)                          │
│    → CSS removed by cleanup                                    │
└─────────────────────────────────────────────────────────────────┘
```

---

## Part 4: Detecting "Navigation Settled"

The key question: **When do we trigger `cleanupOrphans()`?**

### Option A: Debounced Timer After Unregister

```javascript
let cleanupTimer = null;

function unregister(componentId, href) {
    // Remove from refs...
    
    // Schedule cleanup with debounce
    clearTimeout(cleanupTimer);
    cleanupTimer = setTimeout(() => {
        cleanupOrphans();
    }, 100); // 100ms debounce
}
```

**Pros:** Simple, handles rapid unregister/register cycles  
**Cons:** Arbitrary delay, CSS briefly orphaned before cleanup

### Option B: NavigationManager.LocationChanged (C#)

```csharp
public class StyleSheetCleanupService : IDisposable
{
    private readonly IJSRuntime _js;
    private readonly NavigationManager _nav;
    
    public StyleSheetCleanupService(IJSRuntime js, NavigationManager nav)
    {
        _js = js;
        _nav = nav;
        _nav.LocationChanged += OnLocationChanged;
    }
    
    private async void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        // Small delay to let new components register
        await Task.Delay(50);
        await _js.InvokeVoidAsync("bwfc.stylesheetRegistry.cleanupOrphans");
    }
}
```

**Pros:** Explicit navigation trigger  
**Cons:** Doesn't fire in static SSR, requires scoped service

### Option C: MutationObserver in JS

```javascript
const observer = new MutationObserver((mutations) => {
    // Check if any PageStyleSheet placeholder elements were removed
    // Schedule cleanup
});

observer.observe(document.body, { childList: true, subtree: true });
```

**Pros:** Automatic, no C# coordination needed  
**Cons:** Performance overhead, complex to filter relevant mutations

### **Recommendation: Option A (Debounced Timer)**

The debounced timer approach is:
- **Simple** — no coordination between C# and JS
- **Reliable** — works in all render modes
- **Safe** — debounce handles rapid navigations
- **Efficient** — timer only runs when unrefs happen

The delay is invisible to users (CSS is still loaded during the 100ms window).

---

## Part 5: SSR/Interactive Mode Handling

### Static SSR (No JS)

```
Component renders → static <link> tag
Component disposes → no-op (JS not available)
Browser navigation → browser handles CSS cleanup
```

The static `<link>` approach from the current fix remains **unchanged**. The registry only activates when JS is available.

### Prerender → Interactive Transition

```
Prerender: static <link> tag rendered
Hydration: OnAfterRenderAsync fires
           → registry.register() adopts existing <link>
           → registry now manages lifecycle
```

**Key behavior:** `loadStyleSheet()` checks if link already exists. If yes, it adopts it instead of creating a duplicate.

### Interactive-Only (No Prerender)

```
Component renders (no static output in interactive mode)
OnAfterRenderAsync: registry.register() creates <link>
Dispose: registry.unregister()
```

---

## Part 6: Implementation Specification

### JavaScript: `stylesheetRegistry` (in Basepage.module.js)

```javascript
// Stylesheet lifecycle registry
const stylesheetRegistry = {
    // href -> Set<componentId>
    refs: new Map(),
    // href -> HTMLLinkElement
    links: new Map(),
    
    // Cleanup timer handle
    cleanupTimer: null,
    
    /**
     * Register a component's reference to a stylesheet.
     * Creates the <link> element if it doesn't exist.
     */
    register(componentId, href, media, integrity, crossOrigin) {
        // Get or create ref set
        if (!this.refs.has(href)) {
            this.refs.set(href, new Set());
        }
        this.refs.get(href).add(componentId);
        
        // Find existing link or create new
        let link = this.links.get(href);
        if (!link) {
            link = document.querySelector(`link[href="${href}"]`);
            if (link) {
                // Adopt existing static link
                this.links.set(href, link);
                console.debug(`[BWFC] Adopted existing stylesheet: ${href}`);
            } else {
                // Create new link
                link = document.createElement('link');
                link.rel = 'stylesheet';
                link.href = href;
                if (media) link.media = media;
                if (integrity) link.integrity = integrity;
                if (crossOrigin) link.crossOrigin = crossOrigin;
                document.head.appendChild(link);
                this.links.set(href, link);
                console.debug(`[BWFC] Loaded stylesheet: ${href}`);
            }
        }
    },
    
    /**
     * Unregister a component's reference. Schedules cleanup.
     */
    unregister(componentId, href) {
        const refs = this.refs.get(href);
        if (refs) {
            refs.delete(componentId);
            if (refs.size === 0) {
                this.refs.delete(href);
            }
        }
        
        // Schedule cleanup with debounce
        this.scheduleCleanup();
    },
    
    /**
     * Schedule orphan cleanup after navigation settles.
     */
    scheduleCleanup() {
        clearTimeout(this.cleanupTimer);
        this.cleanupTimer = setTimeout(() => {
            this.cleanupOrphans();
        }, 100);
    },
    
    /**
     * Remove stylesheets with no component references.
     */
    cleanupOrphans() {
        for (const [href, link] of this.links.entries()) {
            const refs = this.refs.get(href);
            if (!refs || refs.size === 0) {
                link.remove();
                this.links.delete(href);
                console.debug(`[BWFC] Unloaded orphan stylesheet: ${href}`);
            }
        }
    }
};

// Export for module use
export { stylesheetRegistry };

// Also expose on window for backward compatibility
if (typeof window !== 'undefined') {
    window.bwfc = window.bwfc ?? {};
    window.bwfc.stylesheetRegistry = stylesheetRegistry;
}
```

### C#: Updated PageStyleSheet Component

```csharp
public partial class PageStyleSheet : ComponentBase, IAsyncDisposable
{
    [Inject] private IJSRuntime JS { get; set; } = null!;
    
    [Parameter, EditorRequired] public string Href { get; set; } = "";
    [Parameter] public string? Media { get; set; }
    [Parameter] public string? Id { get; set; }
    [Parameter] public string? Integrity { get; set; }
    [Parameter] public string? CrossOrigin { get; set; }
    
    private string _componentId = "";
    private bool _registered;
    private IJSObjectReference? _module;
    
    private string GetComponentId()
    {
        if (string.IsNullOrEmpty(_componentId))
        {
            _componentId = Id ?? $"bwfc-css-{Guid.NewGuid():N}";
        }
        return _componentId;
    }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && !string.IsNullOrEmpty(Href))
        {
            GetComponentId();
            
            try
            {
                _module = await JS.InvokeAsync<IJSObjectReference>(
                    "import", "./_content/Fritz.BlazorWebFormsComponents/js/Basepage.module.js");
                
                // Register with the global stylesheet registry
                await _module.InvokeVoidAsync(
                    "stylesheetRegistry.register", 
                    _componentId, Href, Media, Integrity, CrossOrigin);
                
                _registered = true;
            }
            catch (InvalidOperationException)
            {
                // SSR-only mode, JS not available
                // Static <link> is already in HTML
            }
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        if (_registered && _module is not null)
        {
            try
            {
                // Unregister from registry (may trigger deferred cleanup)
                await _module.InvokeVoidAsync(
                    "stylesheetRegistry.unregister", 
                    _componentId, Href);
            }
            catch (JSDisconnectedException) { }
            catch (ObjectDisposedException) { }
        }
        
        if (_module is not null)
        {
            try { await _module.DisposeAsync(); }
            catch { }
        }
    }
}
```

### PageStyleSheet.razor (Unchanged for SSR)

```razor
@if (!string.IsNullOrEmpty(Href) && !RendererInfo.IsInteractive)
{
    <link rel="stylesheet" href="@Href" id="@GetComponentId()" 
          media="@Media" integrity="@Integrity" crossorigin="@CrossOrigin" />
}
```

---

## Part 7: Test Scenarios

### Unit Tests (bUnit)

1. **Register on render** — Verify `registry.register` called with correct args
2. **Unregister on dispose** — Verify `registry.unregister` called
3. **Multiple components, same CSS** — Verify ref counting works
4. **Empty Href** — Verify no registration
5. **SSR mode** — Verify static link rendered, no JS calls

### Integration Tests (Playwright)

1. **Enhanced navigation** — Navigate between pages, verify only current page's CSS loaded
2. **Layout CSS persistence** — Navigate between pages, verify layout CSS never unloads
3. **Rapid navigation** — Click multiple links quickly, verify CSS settles correctly
4. **Full page navigation** — Verify browser handles cleanup naturally

---

## Part 8: Migration from Current Implementation

### Breaking Changes

None. The API is identical:
```razor
<PageStyleSheet Href="CSS/Page.css" />
```

### Behavioral Changes

| Scenario | Before | After |
|----------|--------|-------|
| SSR dispose | CSS persists (browser) | CSS persists (browser) |
| Interactive dispose | CSS removed immediately | CSS removed after debounce |
| Layout CSS | Removed on page dispose | Never removed (layout alive) |
| Same CSS, multiple components | Removed on first dispose | Removed when last unregisters |

The behavioral change for "same CSS, multiple components" is a **fix**, not a breaking change.

---

## Part 9: Alternatives Considered

### Alternative A: SectionContent Aggregation

Use Blazor's `SectionContent/SectionOutlet` to aggregate CSS in App.razor.

**Rejected because:**
- Requires App.razor modification
- Steeper learning curve for migrators
- Doesn't help with dynamic load/unload

### Alternative B: C#-Only Registry (No JS State)

Keep all state in a scoped C# service, use JS only for DOM manipulation.

**Rejected because:**
- Scoped services reset on each SSR request
- Can't adopt static `<link>` tags across render modes
- More complex interop

### Alternative C: NavigationManager.LocationChanged Cleanup

Trigger cleanup on C# navigation events instead of JS debounce.

**Rejected because:**
- Doesn't fire in static SSR
- Adds service dependency
- JS debounce is simpler and works everywhere

---

## Part 10: Open Questions

1. **Should cleanup delay be configurable?**
   - Probably not — 100ms is safe for all scenarios.

2. **Should we support explicit "don't unload" marker?**
   - Use case: CDN CSS that should persist forever.
   - Consider: `<PageStyleSheet Href="..." Persistent="true" />`

3. **Should we dedupe by href normalization?**
   - `CSS/Page.css` vs `/CSS/Page.css` vs `./CSS/Page.css`
   - Recommendation: Normalize to absolute URL in JS.

---

## Deliverables for Cyclops

### Files to Create/Modify

1. **`src/BlazorWebFormsComponents/wwwroot/js/Basepage.module.js`**
   - Add `stylesheetRegistry` object with `register`, `unregister`, `cleanupOrphans`
   - Keep existing `loadStyleSheet`/`unloadStyleSheet` as legacy (or deprecate)

2. **`src/BlazorWebFormsComponents/PageStyleSheet.razor.cs`**
   - Replace `_isLoadedViaJs` with `_registered`
   - Replace `loadStyleSheet`/`unloadStyleSheet` calls with `registry.register`/`unregister`
   - Pass `Href` to unregister (registry needs it for ref counting)

3. **`src/BlazorWebFormsComponents.Test/PageStyleSheetTests.cs`**
   - Add tests for registry behavior
   - Add tests for ref counting scenarios

### Implementation Order

1. Add JS `stylesheetRegistry` (additive, doesn't break existing)
2. Update C# component to use registry
3. Add tests
4. Update docs (`dynamic-css-loader-design.md`)

---

## Appendix: Sequence Diagram

```
Page Load (SSR → Interactive)
════════════════════════════

Server                          Browser                         JS Registry
───────                         ───────                         ───────────
 │                                 │                                 │
 │──render PageStyleSheet──────────>│                                 │
 │  <link href="page.css">         │                                 │
 │                                 │                                 │
 │──dispose (no JS interop)────────>│                                 │
 │                                 │                                 │
 │                                 │──blazor hydration──────────────>│
 │                                 │  OnAfterRenderAsync              │
 │                                 │                                 │
 │                                 │──register("css-1", "page.css")─>│
 │                                 │                                 │
 │                                 │                   (adopts <link>)│
 │                                 │                                 │

Enhanced Navigation
═══════════════════

Browser                         JS Registry
───────                         ───────────
 │                                 │
 │──[navigate to /new-page]────────>│
 │                                 │
 │  (old page disposes)            │
 │──unregister("css-1","page.css")─>│
 │                                 │  refs[page.css] = Set()
 │                                 │  scheduleCleanup()
 │                                 │
 │  (new page renders)             │
 │──register("css-2","new.css")────>│
 │                                 │  refs[new.css] = Set(["css-2"])
 │                                 │
 │  (100ms debounce)               │
 │                                 │──cleanupOrphans()
 │                                 │  page.css removed
 │                                 │
```
