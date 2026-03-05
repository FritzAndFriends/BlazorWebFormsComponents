# Forge — Cycle 1 Prioritized Fix List

**Date:** 2026-03-06
**By:** Forge (Lead / Web Forms Reviewer)
**Input:** Run 9 WingtipToys Benchmark Report, Run 9 migrated output, bwfc-migrate.ps1, BWFC component library
**Purpose:** Define exactly what gets fixed in Cycle 1 to maximize the quality of the next migration run (Run 10).

---

## Context

Run 9 achieved 8 functional pages, 173 BWFC control instances, 0 build errors (after 7 iterations), and 667 Layer 1 transforms. The core shopping flow works. However, the report identifies recurring build failures, wasted Layer 2 effort, and missed automation opportunities. This analysis drills into the actual code to produce actionable fix items.

---

## P0 — Must Fix in Cycle 1 (Build Failures Every Run)

### P0-1: ItemType→TItem conversion is backwards for most data controls

**What:** `bwfc-migrate.ps1` line 862–867 blindly converts ALL `ItemType="X"` attributes to `TItem="X"`. Only `DropDownList` uses the `TItem` type parameter. GridView, ListView, FormView, and DetailsView all use `ItemType`. This causes CS0246/CS0535 build errors every run that Layer 2 must manually revert.

**Evidence:** In the Run 9 output, ShoppingCart.razor line 9 uses `ItemType="CartItem"` on GridView (correct), ProductList.razor line 11 uses `ItemType="Product"` on ListView (correct), but AdminPage.razor lines 9 and 55 use `TItem="Category"` / `TItem="Product"` on DropDownList (also correct). Layer 2 had to fix every GridView/ListView/FormView/DetailsView instance.

**Where:** `migration-toolkit/scripts/bwfc-migrate.ps1` lines 862–867

**Fix:** Change the regex to ONLY convert `ItemType→TItem` when the enclosing tag is `<DropDownList`, `<ListBox`, `<CheckBoxList`, `<RadioButtonList`, or `<BulletedList`. For all other BWFC data controls, KEEP `ItemType` as-is.

**Who:** Bishop (script owner)

**Expected impact:** Eliminates the #1 recurring build failure. Saves 1-2 build-fix iterations per run.

---

### P0-2: Stub mechanism discards convertible BWFC markup

**What:** `Test-UnconvertiblePage` (lines 1063–1094) stubs ALL files under `Account/` and `Checkout/` paths, replacing them with empty `@page` + `<h3>not yet migrated</h3>` placeholders. But many of these pages contain valid BWFC controls that Layer 1 CAN mechanically convert. Layer 2 then has to redo ALL markup transforms from scratch.

**Evidence in Run 9 output:**
- `Account/Login.razor` — full BWFC markup (TextBox, CheckBox, Button, RequiredFieldValidator, Label, HyperLink, PlaceHolder, Literal) with empty code-behind. Layer 2 recreated all of this.
- `Account/Register.razor` — full BWFC markup (TextBox, RequiredFieldValidator, CompareValidator, ValidationSummary, Button, Label) with empty code-behind.
- `Account/Confirm.razor` — BWFC markup (PlaceHolder, HyperLink) with empty code-behind.
- `Account/Manage.razor` — BWFC markup (HyperLink, PlaceHolder) with empty code-behind.
- `Checkout/CheckoutReview.razor` — full BWFC markup (GridView, DetailsView, BoundField, TemplateField, Label, Button) with empty code-behind.

All of this markup was converted by Layer 2 from scratch because Layer 1 stubbed it.

**Where:** `migration-toolkit/scripts/bwfc-migrate.ps1` lines 1063–1094, 1176–1186

**Fix:** Change the stub mechanism to a two-part approach:
1. **Always run markup transforms** on ALL .aspx files (including Account/*, Checkout/*).
2. For pages matching unconvertible patterns, **stub only the code-behind** (create a minimal `partial class` with a TODO banner noting Identity/Payment work needed) instead of copying the raw Web Forms code-behind.
3. Remove the `return` at line 1185 that short-circuits the entire transform pipeline.

**Who:** Bishop (script owner)

**Expected impact:** Recovers ~20 pages of mechanical transforms that are currently wasted. Reduces Layer 2 work by ~30 minutes per run for Account/Checkout pages since markup arrives pre-converted.

---

### P0-3: Code-behind `: ComponentBase` base class not stripped

**What:** When code-behinds are copied (lines 1015–1057), the original Web Forms base class (`: Page`, `: System.Web.UI.Page`, or any `: ComponentBase` that Layer 2 adds) can conflict with `@inherits WebFormsPageBase` set globally in `_Imports.razor`. This causes CS0263 "partial declarations must not specify different base classes" every run.

**Where:** `migration-toolkit/scripts/bwfc-migrate.ps1` function `Copy-CodeBehind` (lines 1015–1057)

**Fix:** In `Copy-CodeBehind`, add a regex to strip `: Page`, `: System.Web.UI.Page`, `: System.Web.UI.UserControl`, and `: System.Web.UI.MasterPage` base class declarations from the copied code-behind. Replace with just `: partial class ClassName`. Also strip `using System.Web.*` directives.

**Who:** Bishop (script owner)

**Expected impact:** Eliminates CS0263 build failures. Saves 1 build-fix iteration per run.

---

## P1 — Should Fix in Cycle 1 (Reduces Build Iterations)

### P1-1: Auto-inject validator type parameters

**What:** RequiredFieldValidator and RegularExpressionValidator always need `Type="string"`, CompareValidator always needs `InputType="string"`, RangeValidator always needs `Type="string"`. These are deterministic — the BWFC components require explicit type parameters. Run 9 required adding these to 26 validators during build-fix iteration.

**Evidence:** AdminPage.razor shows validators with `Type="string"` already injected — this was done by Layer 2. Login.razor and Register.razor also show `Type="string"` on validators.

**Where:** `migration-toolkit/scripts/bwfc-migrate.ps1` — new transform step needed after `Remove-WebFormsAttributes`

**Fix:** Add a `Add-ValidatorTypeParameters` function that:
- Injects `Type="string"` into `<RequiredFieldValidator`, `<RegularExpressionValidator`, `<RangeValidator` tags that don't already have a `Type=` attribute.
- Injects `InputType="string"` into `<CompareValidator` tags that don't already have an `InputType=` attribute.

**Who:** Bishop (script owner)

**Expected impact:** Eliminates ~26 build errors per WingtipToys run. Saves 1 build-fix iteration.

---

### P1-2: Strip remaining `<% %>` code blocks in markup

**What:** `ConvertFrom-Expressions` (lines 581–650) handles `<%# %>`, `<%: %>`, `<%= %>`, and `<%-- --%>` expressions, but `<% %>` code blocks (inline C# statements) are only flagged as manual items, not converted or stripped. These cause Razor compilation errors.

**Where:** `migration-toolkit/scripts/bwfc-migrate.ps1` lines 648–655

**Fix:** Convert `<% %>` blocks to `@{ }` Razor code blocks where possible (simple single-line statements), or wrap in `@* TODO: Convert this server-side code block *@` comment for multi-line blocks. At minimum, ensure they don't cause build failures.

**Who:** Bishop (script owner)

**Expected impact:** Reduces build errors in pages with inline code blocks. Run 9 had 0 remaining `<% %>` in output (good), but this is a safety net for other projects.

---

### P1-3: Scaffold `@using BlazorWebFormsComponents.Validations` conditionally

**What:** The report notes this was a build fix item. The current scaffold (`_Imports.razor`) in Run 9 DOES include `@using BlazorWebFormsComponents.Validations` — confirming this was already fixed. However, the script should explicitly check for validators in the source scan and include this namespace automatically rather than always including it.

**Where:** `migration-toolkit/scripts/bwfc-migrate.ps1` — scaffold generation (lines ~100–240)

**Fix:** Already fixed in Run 9's output. Verify the scaffold template includes this namespace. If it's hardcoded, mark as done.

**Who:** Bishop (verify only)

**Expected impact:** Already resolved — verification confirms no regression.

---

### P1-4: Add `ImageButton` to BWFC preservation watchlist

**What:** History notes Run 9 had 2 lost controls, including "ImageButton to img in ShoppingCart (CRITICAL, OnClick lost)". `ImageButton` is described as "a blind spot in Test-BwfcControlPreservation." If Layer 1 or Layer 2 flattens an ImageButton to `<img>`, the `OnClick` event is silently lost.

**Where:** `migration-toolkit/scripts/bwfc-migrate.ps1` — `Test-BwfcControlPreservation` function and `$BwfcComponents` list (line 914–928)

**Fix:** Confirm `ImageButton` IS in the `$BwfcComponents` list (it is, at line 916). The preservation check should already catch this. The real issue may be that Layer 2 agents are flattening it. Add a specific warning in the manual review items: "ImageButton→img flattening loses OnClick event handler."

**Who:** Bishop (script), Cyclops (verify ImageButton component works)

**Expected impact:** Prevents silent loss of click handlers on image buttons.

---

## P2 — Defer to Cycle 2 (BWFC Component Gaps)

### P2-1: GridView editing mode for AdminPage CRUD

**What:** The AdminPage needs full CRUD: add product (form + DropDownList), remove product (DropDownList selection). The current GridView component HAS `EditIndex`, `OnRowEditing`, `OnRowUpdating`, `OnRowDeleting`, `OnRowCancelingEdit` support (confirmed in `GridView.razor.cs` lines 37, 424, 551–620). But the AdminPage doesn't use GridView for editing — it uses standalone form controls. The BWFC components needed are already present; the gap is in Layer 2's implementation of the code-behind.

**Where:** `samples/Run9WingtipToys/Admin/AdminPage.razor` (markup present, code-behind empty), `src/BlazorWebFormsComponents/GridView.razor.cs`

**Fix:** No BWFC component work needed. Layer 2 needs to implement the AdminPage code-behind with EF Core CRUD operations. The DropDownList `SelectMethod` TODO pattern is correct.

**Who:** Bishop (Layer 2 skill improvement for CRUD pages)

**Expected impact:** Promotes AdminPage from stub → functional. Adds 1 functional page.

---

### P2-2: DropDownList data binding for `SelectMethod` / `AppendDataBoundItems`

**What:** AdminPage.razor uses `<DropDownList AppendDataBoundItems="true" DataTextField="CategoryName" DataValueField="CategoryID">` — the BWFC DropDownList supports `Items`, `DataTextField`, `DataValueField`, but `AppendDataBoundItems` behavior (appending to a pre-populated list) may not be implemented.

**Where:** `src/BlazorWebFormsComponents/DropDownList.razor.cs`

**Who:** Cyclops (verify and implement if missing)

**Expected impact:** Enables correct dropdown behavior on AdminPage.

---

### P2-3: `OpenAuthProviders` is not a BWFC component

**What:** `Account/Login.razor` line 57 references `<OpenAuthProviders ID="OpenAuthLogin" />`. This is a WingtipToys user control (`.ascx`), not an ASP.NET built-in control. It was converted as a component with an empty code-behind. It compiles (partial class exists) but does nothing.

**Where:** `samples/Run9WingtipToys/Account/OpenAuthProviders.razor.cs`

**Fix:** This is expected — user controls become empty Blazor components until implemented. No BWFC work needed. Layer 2 could implement OAuth providers using ASP.NET Core's `ExternalLogins` pattern, but that's an Identity concern.

**Who:** N/A (deferred to Identity implementation)

**Expected impact:** None until Identity work is done.

---

### P2-4: `Manage.razor` HyperLink dropped warning from Run 9

**What:** History mentions "HyperLink dropped in Manage (MINOR)". Looking at the actual `Account/Manage.razor`, it has `<HyperLink NavigateUrl="..." Text="[Change]">` etc. — these are present as BWFC components. The "dropped" warning may be stale or was a Layer 2 regression that was subsequently fixed.

**Where:** `samples/Run9WingtipToys/Account/Manage.razor`

**Fix:** Verify during next run. No action needed if HyperLinks are present.

**Who:** Forge (verify in Run 10)

---

## P3 — Defer to Cycle 3 (Larger Scope)

### P3-1: ASP.NET Identity scaffolding for 15 Account pages

**What:** 15 Account pages (Login, Register, Manage, Forgot, ResetPassword, Confirm, etc.) have correct BWFC markup but empty code-behinds. They need ASP.NET Core Identity wired up: `UserManager<T>`, `SignInManager<T>`, cookie auth, email confirmation flow.

**Who:** Bishop (Layer 2 skill for Identity scaffold template)

**Expected impact:** Promotes 15 pages from stub → functional.

---

### P3-2: PayPal checkout integration for 5 Checkout pages

**What:** CheckoutStart, CheckoutReview, CheckoutComplete, CheckoutCancel, CheckoutError. CheckoutReview has full BWFC markup (GridView, DetailsView) but needs a checkout service.

**Who:** Out of scope for BWFC — application-level concern.

---

### P3-3: DB-backed cart persistence

**What:** `CartStateService` is scoped in-memory. Web Forms WingtipToys uses database-backed cart via `UsersShoppingCart` table. For production migrations, cart should survive page refreshes.

**Who:** Bishop (service template improvement)

---

## Summary: Cycle 1 Assignments

| ID | Priority | Assignee | Item | Expected Impact |
|----|----------|----------|------|-----------------|
| P0-1 | **P0** | Bishop | Fix ItemType→TItem (only DropDownList uses TItem) | Eliminates #1 build failure |
| P0-2 | **P0** | Bishop | Smart stub: convert markup, stub code-behind only | Recovers ~20 pages of transforms |
| P0-3 | **P0** | Bishop | Strip Web Forms base classes from code-behinds | Eliminates CS0263 errors |
| P1-1 | **P1** | Bishop | Auto-inject validator Type="string" / InputType="string" | Eliminates ~26 build errors |
| P1-2 | **P1** | Bishop | Convert/strip remaining `<% %>` code blocks | Safety net for non-WingtipToys projects |
| P1-3 | **P1** | Bishop | Verify Validations namespace in scaffold (likely done) | Verify only |
| P1-4 | **P1** | Bishop+Cyclops | ImageButton preservation warning | Prevents silent OnClick loss |
| P2-1 | P2 | Bishop | AdminPage CRUD code-behind | +1 functional page |
| P2-2 | P2 | Cyclops | DropDownList AppendDataBoundItems | Admin dropdown behavior |
| P2-3 | P2 | N/A | OpenAuthProviders (user control) | Deferred to Identity |
| P2-4 | P2 | Forge | Verify Manage.razor HyperLink | Verify in Run 10 |
| P3-1 | P3 | Bishop | Identity scaffolding (15 pages) | +15 functional pages |
| P3-2 | P3 | N/A | PayPal checkout | Out of BWFC scope |
| P3-3 | P3 | Bishop | DB-backed cart | Production readiness |

**Cycle 1 target: Fix P0-1, P0-2, P0-3, P1-1, P1-4. Verify P1-3. Re-migrate. Expect: 0 build errors on first attempt, ~20 more pages with mechanical markup, 2-3 fewer build-fix iterations.**
