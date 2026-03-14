# L1 Script Fixes — Run 22 Improvements

**Date:** 2026-03-14  
**Author:** Cyclops (Component Dev)  
**Context:** ContosoUniversity migration benchmark Run 22 achieved 39/40 tests (97.5% pass rate). Analysis identified 5 script improvements to eliminate remaining warnings and test failures.

## Decisions

### 1. ContentTemplate Wrapper Stripping

**Decision:** Strip `<ContentTemplate>` and `</ContentTemplate>` tags in L1 script after asp: prefix removal.

**Rationale:**
- UpdatePanel's ContentTemplate is a Web Forms wrapper concept — in Blazor, child content goes directly inside the component
- Leaving these tags generates RZ10012 Blazor compiler warnings
- The BWFC UpdatePanel component supports both Web Forms syntax (with ContentTemplate parameter) and Blazor syntax (with ChildContent)
- Stripping the wrapper tags produces cleaner L1 output that works immediately without warnings

**Implementation:** Added regex replacement in `ConvertFrom-AspPrefix` function after closing tag processing.

---

### 2. Dual Route for Home Pages

**Decision:** Auto-generate both `@page "/Home"` and `@page "/"` directives for home page files.

**Rationale:**
- Web Forms apps commonly use Home.aspx, Default.aspx, or Index.aspx as the root page
- Blazor needs explicit `@page "/"` directive for root URL routing
- Tests expect the app root (/) to route to the home page
- Adding both routes ensures the page is accessible via both /Home and / URLs
- Detection pattern: Home.aspx, Default.aspx, Index.aspx (case-insensitive)

**Implementation:** Added `$isHomePage` detection in `ConvertFrom-PageDirective`, generates second `@page "/"` directive when applicable.

---

### 3. PageTitle from ContentPlaceHolderID

**Decision:** Extract page title from `<asp:Content ContentPlaceHolderID="TitleContent">` blocks and generate `<PageTitle>` component.

**Rationale:**
- Web Forms pages set titles via `<asp:Content ContentPlaceHolderID="TitleContent">` or `Title="..."` attribute
- L1 script already extracts Title attribute from `<%@ Page Title="..." %>`
- Missing extraction from TitleContent placeholders — need parity
- Blazor requires `<PageTitle>` component for browser tab title
- Extraction order: Title attribute first, TitleContent placeholder second (if Title attribute absent)

**Implementation:**
- Added regex in `ConvertFrom-ContentWrappers` to extract title text from TitleContent placeholders
- Stored extracted title in script-scoped variable `$script:ExtractedTitleFromContent`
- `ConvertFrom-PageDirective` uses extracted title as fallback if `Title` attribute not present
- Existing code-behind detection logic (skips if `Page.Title =` found) still applies

---

### 4. ID Attribute Normalization

**Decision:** Convert `ID="value"` to `id="value"` on all elements in L1 output.

**Rationale:**
- Web Forms uses `ID` attribute (capital letters) as server-side control identifier
- HTML standard uses lowercase `id` attribute for element identification
- Test selectors (Playwright) look for HTML `id` attributes
- BWFC components accept both `ID` (as parameter) and `id` (rendered to HTML) — broad replacement is safe
- Ensures migrated pages work with existing CSS and JavaScript selectors

**Implementation:** Added regex replacement in `Remove-WebFormsAttributes` function after ItemType processing.

---

### 5. EDMX Computed Properties

**Decision:** Generate `[DatabaseGenerated(DatabaseGeneratedOption.Computed)]` annotation for properties with `StoreGeneratedPattern="Computed"`.

**Rationale:**
- EF6 EDMX files support `StoreGeneratedPattern` attribute with values: Identity, Computed, None
- L1 script already handles Identity pattern
- Computed pattern (e.g., calculated columns, timestamps) was missing
- EF Core requires explicit `[DatabaseGenerated(DatabaseGeneratedOption.Computed)]` annotation
- Missing annotation causes EF Core to attempt INSERT/UPDATE of computed columns → SQL errors

**Implementation:**
- Added `IsComputed` property to entity property metadata in EDMX parser
- Added annotation generation in entity file output (Convert-EdmxToEfCore.ps1)
- Mirrors existing IsIdentity pattern

---

## Impact

**Expected improvements:**
- Fix 1: Eliminates 3 RZ10012 warnings (ContentTemplate in 3 pages)
- Fix 2: Fixes 3 HomePageTests (all test root URL routing)
- Fix 3: Improves SEO and user experience (proper browser tab titles)
- Fix 4: Fixes CSS/JS selector failures from ID casing mismatch
- Fix 5: Prevents EF Core runtime errors on EDMX-generated entities with computed properties

**Backward compatibility:**
- All fixes are additive or normalization — no breaking changes
- Existing L1 output behavior preserved where not explicitly changed

---

## Alternatives Considered

### Fix 1: ContentTemplate handling
- **Alt:** Leave ContentTemplate tags, update BWFC component to accept them as child elements
  - **Rejected:** Adds unnecessary complexity to component, generates warnings
- **Alt:** Handle in L2 only
  - **Rejected:** L1 should produce warning-free output when possible

### Fix 2: Root route
- **Alt:** Only generate `@page "/"` for Default.aspx/Index.aspx
  - **Rejected:** Home.aspx is equally common as root page name
- **Alt:** Manually fix in L2
  - **Rejected:** L1 can detect this pattern reliably

### Fix 4: ID attribute
- **Alt:** Target only HTML elements (div, span, input, etc.) with specific tag list
  - **Rejected:** BWFC components accept both, broad replacement is safer and simpler
- **Alt:** Leave as-is, handle in L2
  - **Rejected:** L1 normalization produces cleaner output

---

**Status:** ✅ Implemented  
**Files Modified:**
- `migration-toolkit/scripts/bwfc-migrate.ps1` (Fixes 1-4)
- `migration-toolkit/scripts/Convert-EdmxToEfCore.ps1` (Fix 5)
