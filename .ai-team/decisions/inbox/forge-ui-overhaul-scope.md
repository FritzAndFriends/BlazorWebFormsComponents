# Sample Website UI Overhaul — Scope & Work Breakdown

**Author:** Forge  
**Date:** 2026-02-13  
**Requested by:** Jeffrey T. Fritz

---

## 1. Current State Analysis

### Layout Structure
- **MainLayout.razor:** Classic sidebar + main content layout
  - Fixed 250px sidebar (purple gradient background, sticky)
  - Top row with Docs/About links
  - Main content area with `@Body`
- **NavMenu.razor:** Uses `TreeView` component for navigation (176 lines of hardcoded TreeNode markup)
  - Categories: Home → Utility Features → Editor → Data → Validation → Navigation → Login → Migration Guides
  - No search functionality
  - TreeView is nested 3-4 levels deep — complex to navigate

### CSS Framework
- **Bootstrap 4.3.1** (2019 vintage — two major versions behind)
- Custom `site.css` (~200 lines) for layout, sidebar theming, validation states
- Open Iconic icon font (Bootstrap 4 era icons)
- No utility-first CSS — all custom classes

### Sample Page Organization
- **34 top-level component folders** in `Components/Pages/ControlSamples/`
- Pattern: Each component folder contains `Index.razor` + variant pages + `Nav.razor` for sub-navigation
- No consistent structure — some have 1 page, some have 6+
- `ComponentList.razor` on homepage shows flat list by category — **manually maintained, out of sync** (missing DetailsView, PasswordRecovery links)

### Static Assets
- `wwwroot/css/` — Bootstrap + site.css
- `wwwroot/img/` — Sample images for AdRotator, Chart
- No favicon customization, no branding assets

### Integration Tests
- **4 test files:** `ControlSampleTests.cs`, `InteractiveComponentTests.cs`, `HomePageTests.cs`, `PlaywrightFixture.cs`
- Tests use **semantic selectors** (element types, attributes) not CSS class selectors
- Example: `page.Locator("span[style*='font-weight']")`, `page.QuerySelectorAsync("canvas")`
- **Low risk from CSS changes** — tests don't depend on `.sidebar`, `.page`, etc.

---

## 2. Proposed Design Direction

### 2.1 Layout Structure

**Recommendation: Modern sidebar + card-based demo area**

```
┌────────────────────────────────────────────────────────────┐
│ [Logo] BlazorWebFormsComponents    [Search: ______] [Docs]│
├─────────────┬──────────────────────────────────────────────┤
│ NAVIGATION  │  BREADCRUMB: Home > Data Controls > GridView│
│             ├──────────────────────────────────────────────┤
│ [Search 🔍] │  ┌─────────────────────────────────────────┐ │
│             │  │ GridView                                │ │
│ ▼ Editor    │  │ ─────────────────────────────────────── │ │
│   Button    │  │ Description text from component docs   │ │
│   CheckBox  │  └─────────────────────────────────────────┘ │
│   ...       │                                              │
│ ▼ Data      │  ┌─────────────────────────────────────────┐ │
│   GridView ←│  │ Live Demo                               │ │
│   ListView  │  │ ┌─────────────────────────────────────┐ │ │
│   ...       │  │ │  <actual component renders here>   │ │ │
│ ▼ Validation│  │ └─────────────────────────────────────┘ │ │
│ ▼ Navigation│  └─────────────────────────────────────────┘ │
│ ▼ Login     │                                              │
│             │  ┌─────────────────────────────────────────┐ │
│             │  │ Code Example                   [Copy 📋]│ │
│             │  │ <pre><code>...</code></pre>            │ │
│             │  └─────────────────────────────────────────┘ │
│             │                                              │
│             │  ┌──────┐ ┌──────┐ ┌──────┐                 │
│             │  │Style │ │Events│ │Paging│  ← sub-pages    │
│             │  └──────┘ └──────┘ └──────┘                 │
└─────────────┴──────────────────────────────────────────────┘
```

**Key changes:**
1. **Persistent top bar** with search input + branding
2. **Collapsible sidebar** with category grouping (current TreeView → simple `<details>` or Blazor Accordion)
3. **Card-based demo pages** — description card, live demo card, code example card
4. **Sub-page tabs** — replace current `Nav.razor` pattern with horizontal tabs

### 2.2 CSS Approach

**Recommendation: Bootstrap 5.3 (latest stable)**

| Option | Pros | Cons | Verdict |
|--------|------|------|---------|
| **Bootstrap 5.3** | Familiar to team, minimal learning curve, great docs, no jQuery | Needs migration from 4.3 classes | ✅ **RECOMMENDED** |
| Tailwind CSS | Modern, utility-first | Build tooling, different paradigm | ❌ Overkill for sample site |
| FluentUI Blazor | Microsoft ecosystem | Heavy dependency, learning curve | ❌ Different library, confusing |
| Custom CSS only | Full control | Maintenance burden, no responsive grid | ❌ Not worth it |

**Bootstrap 4→5 breaking changes to address:**
- `ml-*` → `ms-*`, `mr-*` → `me-*` (margin utilities)
- `pl-*` → `ps-*`, `pr-*` → `pe-*` (padding utilities)
- `data-toggle` → `data-bs-toggle` (JS attributes — not used)
- `form-group` → `mb-3` (form layout)
- `.close` → `.btn-close` (close buttons)
- No jQuery dependency (already not using it)

### 2.3 Component Organization

**Current:** Flat navigation duplicated in TreeView (NavMenu) + ComponentList + manual sample pages

**Proposed:**
1. **Single source of truth:** `ComponentCatalog.json` or static class with component metadata:
   ```json
   {
     "components": [
       {
         "name": "Button",
         "category": "Editor",
         "route": "/ControlSamples/Button",
         "description": "Server-side button control",
         "subPages": ["Style", "Events", "JavaScript"]
       }
     ]
   }
   ```
2. **Auto-generate NavMenu** from catalog
3. **Auto-generate ComponentList** from catalog
4. **Template-driven sample pages** — reduce boilerplate

### 2.4 Search Implementation

**Recommendation: Client-side search with Fuse.js or similar**

| Approach | Pros | Cons | Verdict |
|----------|------|------|---------|
| **Client-side JS (Fuse.js)** | Zero server load, instant results, works offline | 50KB+ bundle, client rendering | ✅ **RECOMMENDED** |
| Blazor input + filter | No JS, type-safe | Re-renders on every keystroke | ⚠️ Viable fallback |
| Server-side API | Scalable | Overkill for <100 pages | ❌ Unnecessary |

**Implementation:**
1. Generate `search-index.json` at build time from component catalog
2. Include component name, category, description, keywords
3. Fuse.js fuzzy search with highlighting
4. Results show in dropdown below search input
5. Keyboard navigation (arrow keys + Enter)

---

## 3. Work Breakdown

| ID | Title | Owner | Size | Dependencies | Notes |
|----|-------|-------|------|--------------|-------|
| UI-1 | Upgrade Bootstrap 4.3→5.3 | Jubilee | M | — | Replace CSS files, update utility classes in site.css |
| UI-2 | Create ComponentCatalog data source | Cyclops | S | — | JSON or static class with all 50+ components |
| UI-3 | Redesign MainLayout.razor | Jubilee | M | UI-1 | New layout structure, top bar, breadcrumbs |
| UI-4 | Redesign NavMenu from catalog | Jubilee | M | UI-2, UI-3 | Replace TreeView with Bootstrap 5 Accordion |
| UI-5 | Create SamplePageTemplate | Jubilee | M | UI-3 | Card layout: description, demo, code, sub-tabs |
| UI-6 | Migrate sample pages to template | Jubilee | L | UI-5 | 34 component folders, ~80 pages total |
| UI-7 | Update ComponentList.razor | Jubilee | S | UI-2 | Generate from catalog, add missing components |
| UI-8 | Implement search (Fuse.js) | Cyclops | M | UI-2 | Index generation, search component, dropdown |
| UI-9 | Update integration tests | Colossus | M | UI-3, UI-4 | Verify all routes, update any broken selectors |
| UI-10 | Add dark mode toggle | Jubilee | S | UI-1 | Bootstrap 5 color modes, localStorage persistence |
| UI-11 | Update branding/favicon | Beast | S | — | BlazorWebFormsComponents logo, favicon.ico |
| UI-12 | Documentation for new layout | Beast | S | UI-6 | Update any docs referencing sample site |

### Dependency Graph

```
         ┌──────┐
         │ UI-1 │ Bootstrap upgrade (Jubilee)
         └──┬───┘
            │
    ┌───────┼────────┐
    ▼       ▼        ▼
┌──────┐ ┌──────┐ ┌──────┐
│ UI-2 │ │ UI-3 │ │UI-10 │
│Catalog│ │Layout│ │Dark  │
│(Cyc)  │ │(Jub) │ │Mode  │
└──┬───┘ └──┬───┘ └──────┘
   │        │
   ├────────┤
   ▼        ▼
┌──────┐ ┌──────┐
│ UI-4 │ │ UI-5 │
│NavMenu│ │Templat│
│(Jub)  │ │(Jub)  │
└──┬───┘ └──┬───┘
   │        │
   │        ▼
   │     ┌──────┐
   │     │ UI-6 │
   │     │Migrate│
   │     │(Jub)  │
   │     └──┬───┘
   │        │
   ├────────┤
   ▼        ▼
┌──────┐ ┌──────┐
│ UI-7 │ │ UI-8 │
│CompLst│ │Search│
│(Jub)  │ │(Cyc) │
└──────┘ └──┬───┘
            │
            ▼
         ┌──────┐
         │ UI-9 │
         │Tests │
         │(Col) │
         └──────┘
```

### Parallel Execution Plan

**Phase 1 (parallel):**
- UI-1: Bootstrap upgrade
- UI-2: ComponentCatalog
- UI-11: Branding

**Phase 2 (after Phase 1):**
- UI-3: MainLayout redesign
- UI-10: Dark mode

**Phase 3 (after Phase 2):**
- UI-4: NavMenu
- UI-5: SamplePageTemplate

**Phase 4 (after Phase 3):**
- UI-6: Migrate pages (largest item)
- UI-7: ComponentList
- UI-8: Search

**Phase 5 (after Phase 4):**
- UI-9: Integration tests
- UI-12: Documentation

---

## 4. Risk Assessment

### 4.1 Integration Test Breakage Risk: **LOW**

Current tests use:
- Element selectors: `button`, `input[type='submit']`, `canvas`, `table`, `a`, `li`
- Attribute selectors: `span[style*='font-weight']`, `img[src='/img/CSharp.png']`
- ID selectors: `#event-count`, `#event-details`
- Class selectors: `.item-row`, `.alt-item-row` (component output, not layout)

**No layout CSS class selectors found in tests.** Tests target component output, not page structure.

**Mitigation:** UI-9 (Colossus) runs full test suite after each major phase. Fix any breakage immediately.

### 4.2 Hardcoded Selectors: **MEDIUM**

Found hardcoded patterns:
- `NavMenu.razor` line 6: `navbar-brand` class (Bootstrap 4)
- `ComponentList.razor` line 66: `col-md=3` (typo! should be `col-md-3`)
- `site.css` references `.sidebar`, `.page`, `.main`, `.top-row`

**Mitigation:** UI-3 (MainLayout) and UI-4 (NavMenu) will replace these classes. Grep for all Bootstrap 4 class usages before Phase 2.

### 4.3 Search Implementation: **MEDIUM**

Client-side search requires:
1. JS interop for Fuse.js (first non-Chart JS in sample app)
2. Build-time index generation (manual or automated)
3. Keyboard navigation UX

**Mitigation:** 
- Use existing JS interop patterns from Chart component
- Start with manual index; automate later if needed
- Keep scope to basic dropdown; no fancy UX

### 4.4 Large Migration Scope: **HIGH**

UI-6 touches ~80 files across 34 component folders. Risk of:
- Inconsistent migration
- Broken links
- Lost sample code

**Mitigation:**
- Create template first (UI-5)
- Migrate 2-3 components as pilot (Button, GridView, Calendar)
- Review pilot with Jeff before proceeding
- Use checklist to track progress

### 4.5 Bootstrap 4→5 Breaking Changes: **LOW**

Most changes are utility class renames. No jQuery dependency to remove.

**Mitigation:** 
- Run `grep -r "ml-\|mr-\|pl-\|pr-"` to find all usages
- Batch replace with `ms-`/`me-`/`ps-`/`pe-`
- Verify responsive behavior after upgrade

---

## 5. Open Questions for Jeff

1. **Dark mode priority?** UI-10 is nice-to-have. Include in Phase 2 or defer?
2. **Search scope?** Component names only, or also search within docs/descriptions?
3. **Branding assets?** Do you have a BlazorWebFormsComponents logo, or should Beast create one?
4. **Migration guide updates?** Should we update the MasterPages migration guide to reference the new layout?

---

## 6. Recommendation

**Proceed with UI-1, UI-2, UI-11 in parallel immediately.** These are foundational and have no dependencies.

**Estimated total effort:** 3-4 sprints (assuming 2-day sprints)
- Phase 1-2: 1 sprint
- Phase 3: 1 sprint
- Phase 4: 1-2 sprints (UI-6 is large)
- Phase 5: 0.5 sprint

**Owners:**
- Jubilee: UI-1, UI-3, UI-4, UI-5, UI-6, UI-7, UI-10 (frontend lead)
- Cyclops: UI-2, UI-8 (catalog + search logic)
- Colossus: UI-9 (integration tests)
- Beast: UI-11, UI-12 (branding + docs)
