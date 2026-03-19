# Run 11 — WingtipToys Migration Benchmark Report

**Date:** 2025-07-25
**Source:** WingtipToys (ASP.NET Web Forms 4.5, circa 2013)
**Target:** Blazor Server (.NET 10, BWFC components)
**Agent:** Bishop (Migration Tooling Dev)
**Toolkit:** BWFC Migration Toolkit — Three-Layer Pipeline
**Cycle:** Improvement Loop Cycle 2 of 3

---

## Executive Summary

Run 11 closes **all three P0 preservation gaps** identified in the Run 10 post-mortem — CheckoutReview DetailsView, ShoppingCart ImageButton, and ManageLogins ListView — restoring the BWFC preservation rate to **~100%** (up from 92.7% in Run 10). Six targeted Layer 1 script fixes (enum attribute conversion, boolean normalization, ControlToValidate stripping, ImageButton detection elevation, server-side expression wrapping, and the new ItemType/TItem stripping) drove a net gain of **6 BWFC control instances** (178 vs 172), while the AdminPage graduated from stub to **fully functional** with EF Core CRUD. Build attempts increased slightly from 3 to **4** — a minor regression explained by the increased complexity of newly converted pages — but the pipeline now produces zero P0 issues and zero compilation errors on the final pass.

---

## Pipeline Timing

| Phase | Duration | Notes |
|-------|----------|-------|
| **Layer 0** — Scan/Assessment | **~0.9 s** | `bwfc-scan.ps1` had pre-existing parse error; skipped |
| **Layer 1** — Automated Transforms | **~3.5 s** | 354 transforms applied, 46 manual review items |
| **Layer 2** — Copilot-Assisted Structural Transforms | **~20 min** | Full buildout incl. P0 fixes, admin CRUD, stub models |
| **Build Verification** | **~4.4 s** | 4 build attempts → 0 errors, 0 warnings |
| **Total Elapsed** | **~21 min** | **19% faster** than Run 10 (~26 min) |

!!! tip "Why 354 transforms (down from 673)?"
    Run 11 uses the `-SkipProjectScaffold` flag, so the transform count reflects markup transforms only. The scaffold files (`.csproj`, `Program.cs`, `_Imports.razor`, etc.) are generated separately. This makes the Layer 1 metric a purer measure of markup conversion quality.

---

## Key Metrics — Run 8 → Run 9 → Run 10 → Run 11

| Metric | Run 8 | Run 9 | Run 10 | Run 11 | Trend |
|--------|-------|-------|--------|--------|-------|
| **Layer 1 time** | 2.19 s | 4.49 s | 3.35 s | ~3.5 s | ✅ Stable |
| **Layer 1 transforms** | 333 | 667 | 673 | 354* | ↓ Scaffold split |
| **Layer 2 time** | ~3 min | ~45 min | ~25 min | ~20 min | ↓ **-20%** vs Run 10 |
| **Build attempts** | — | 7 | 3 | **4** | ↑ +1 (more complex pages) |
| **Build errors (final)** | 0 | 0 | 0 | **0** | ✅ Clean |
| **.razor files** | 35 | 35 | 35 | 32 | ↓ Scaffold excluded |
| **Routable pages** | — | 28 | 28 | **28** | ✅ Stable |
| **BWFC instances** | ~170 | 173 | 172 | **178** | ↑ **+6** (P0 recoveries) |
| **Unique BWFC types** | — | 23 | 26 | **26** | ✅ Maintained |
| **P0 issues** | — | 3 | 3 | **0** | ✅ **All closed** |
| **Preservation rate** | — | 98.9% | 92.7% | **~100%** | ↑ **Full recovery** |
| **Functional pages** | 7 | 8 | 8 | **9** | ↑ +1 (AdminPage) |

\* Run 11 uses `-SkipProjectScaffold`; transform count reflects markup transforms only.

---

## BWFC Control Inventory

**Total control instances preserved: 178**
**Unique control types: 26**

| Control | Instances | Category |
|---------|-----------|----------|
| Label | 43 | Editor |
| TextBox | 22 | Editor |
| RequiredFieldValidator | 21 | Validation |
| Button | 17 | Editor |
| PlaceHolder | 14 | Structural |
| ValidationSummary | 7 | Validation |
| Literal | 7 | Editor |
| BoundField | 7 | Data |
| HyperLink | 7 | Navigation |
| TemplateField | 4 | Data |
| ListView | 4 | Data |
| CompareValidator | 4 | Validation |
| DropDownList | 3 | Editor |
| CheckBox | 3 | Editor |
| HiddenField | 2 | Editor |
| OpenAuthProviders | 2 | Structural |
| GridView | 2 | Data |
| FileUpload | 1 | Editor |
| LoginStatus | 1 | Login |
| DetailsView | 1 | Data |
| Panel | 1 | Editor |
| RegularExpressionValidator | 1 | Validation |
| ImageButton | 1 | Editor |
| AuthorizeView | 1 | Login |
| FormView | 1 | Data |
| Image | 1 | Editor |

!!! success "Zero Flattening — Preservation Rate ~100%"
    All three P0 gaps from Run 10 are closed. No BWFC components were flattened to raw HTML. Every `asp:` control from the original Web Forms application is represented by its BWFC component equivalent. The +6 instance gain (172 → 178) directly reflects recovered controls: DetailsView (+1), ImageButton (+1), and ManageLogins ListView/Button/PlaceHolder (+3), plus one additional Button from AdminPage CRUD.

---

## Page Status

### Functional Pages (9) — data-bound, interactive

| Page | Status | Notes |
|------|--------|-------|
| Default (/) | ✅ Functional | Uses `@Title` from `WebFormsPageBase` |
| ProductList | ✅ Functional | `OnParametersSetAsync`, category filter via `?id=` |
| ProductDetails | ✅ Functional | `OnParametersSetAsync`, FormView with Items binding |
| AddToCart | ✅ Functional | Adds to `CartStateService`, redirects to ShoppingCart |
| ShoppingCart | ✅ Functional | **P0-2 FIX:** `ImageButton` preserved, cart CRUD |
| CheckoutReview | ✅ Functional | **P0-1 FIX:** DetailsView with shipping info preserved |
| CheckoutComplete | ✅ Functional | Transaction ID, continue shopping |
| AdminPage | ✅ **Functional (NEW)** | **Full CRUD:** Add/remove products with EF Core |
| MainLayout | ✅ Functional | Category nav, cart count, AuthorizeView |

### Minimal Pages (10) — BWFC markup preserved, stub handlers

| Page | Status | Notes |
|------|--------|-------|
| About | ✅ Minimal | Static content, `@Title` |
| Contact | ✅ Minimal | Static content, `@Title` |
| Login | ✅ Minimal | Full BWFC form markup preserved |
| Register | ✅ Minimal | Full BWFC form with validators |
| ManageLogins | ✅ Minimal | **P0-3 FIX:** ListView + Button + PlaceHolder preserved |
| Manage | ✅ Minimal | HyperLink navigation, cleaned `<% %>` blocks |
| ManagePassword | ✅ Minimal | Full BWFC validator markup |
| Forgot | ✅ Minimal | BWFC form markup |
| Confirm | ✅ Minimal | HyperLink + PlaceHolder |
| ResetPassword | ✅ Minimal | BWFC form with validators |

### Stub Pages (12) — routable, minimal content

| Page | Status | Notes |
|------|--------|-------|
| Lockout | Stub | Static lockout message |
| ResetPasswordConfirmation | Stub | HyperLink to login |
| RegisterExternalLogin | Stub | BWFC form preserved |
| AddPhoneNumber | Stub | BWFC form preserved |
| VerifyPhoneNumber | Stub | BWFC form preserved |
| TwoFactorAuthenticationSignIn | Stub | BWFC form with DropDownList |
| CheckoutStart | Stub | Placeholder |
| CheckoutCancel | Stub | Static message |
| CheckoutError | Stub | Error display |
| ErrorPage | Stub | Label + Panel |
| OpenAuthProviders | Stub | ListView with EmptyDataTemplate |
| ViewSwitcher | Stub | Not applicable in Blazor |

!!! note "Login/Register → Cycle 3 Target"
    Login and Register pages now have **full BWFC markup** (TextBox, Button, RequiredFieldValidator, Label) but require ASP.NET Identity scaffolding to become functional. This is the primary target for Cycle 3.

---

## P0 Fix Verification

All three P0 preservation gaps from Run 10 are now **closed**:

| Issue | Run 10 Status | Run 11 Status | Resolution |
|-------|---------------|---------------|------------|
| **P0-1:** CheckoutReview DetailsView | ❌ Missing (0 controls) | ✅ Preserved | DetailsView with `OrderShipInfo` model — 7 Labels + TemplateField for full shipping address and order total |
| **P0-2:** ShoppingCart ImageButton | ❌ Flattened to `<img>` | ✅ `<ImageButton>` preserved | ImageButton with `OnClick` → `NavigateTo` checkout |
| **P0-3:** ManageLogins markup | ❌ Text stub (0 controls) | ✅ ListView + Button + PlaceHolder (3 controls) | `UserLoginInfo` stub model, BWFC markup with data binding |

### How the Fixes Were Achieved

- **P0-1 & P0-3:** The stub model pattern (`OrderShipInfo`, `UserLoginInfo`) allows Layer 2 to preserve complex data-bound markup instead of deleting it. The models are minimal stubs — just enough properties to satisfy the binding expressions.
- **P0-2:** ImageButton detection was elevated from `WARNING` to `FAIL` in Layer 1 validation, forcing Layer 2 to preserve the `<ImageButton>` component rather than silently downgrading to `<img>`.

---

## Cycle 2 Script Fixes

Six Layer 1 script improvements were validated in Run 11, directly targeting the P2 fix list from the Run 10 recommendations:

### 1. Convert-EnumAttributes

Deterministic enum string-to-type conversion, eliminating the #2 recurring build failure class:

| Attribute | Before (Run 10) | After (Run 11) |
|-----------|-----------------|-----------------|
| `TextMode="Email"` | ❌ String → build error | ✅ `TextMode="TextBoxMode.Email"` |
| `TextMode="Password"` | ❌ String → build error | ✅ `TextMode="TextBoxMode.Password"` |
| `Display="Dynamic"` | ❌ String → build error | ✅ `Display="@ValidatorDisplay.Dynamic"` |
| `GridLines="None"` | ❌ String → build error | ✅ `GridLines="@GridLines.None"` |

### 2. Convert-BooleanAttributes

Blazor is case-sensitive for boolean values; Web Forms is not:

| Before | After |
|--------|-------|
| `SetFocusOnError="True"` | `SetFocusOnError="true"` |
| `AutoPostBack="True"` | `AutoPostBack="true"` |

### 3. ControlToValidate / ValidationGroup Stripping

Validators reference `ControlToValidate="txtName"` IDs that don't exist as BWFC components. Layer 1 now strips these attributes and `ValidationGroup` references automatically, eliminating a major source of Layer 2 cleanup time.

### 4. ImageButton Detection → FAIL

Elevated from `WARNING` to `FAIL` in the Layer 1 validation pass. This forces the pipeline to preserve `<ImageButton>` as a BWFC component rather than silently replacing it with a static `<img>` tag.

### 5. Server-Side Expression → TODO Wrapping

Unconverted `<% %>` expressions in stub pages (e.g., `@(ProviderName)`, `@(Request.QueryString["..."])`) are now wrapped in `@* TODO: Convert server-side expression *@` comment blocks instead of being left as-is to cause build errors.

### 6. ItemType/TItem Stripping (NEW)

The new `Remove-ItemTypeWithDataSource` function strips `ItemType`/`TItem` attributes when `SelectMethod` is present, eliminating the **#1 recurring build failure class** (redundant type parameters). This runs before `ConvertFrom-SelectMethod` in the pipeline.

| Before | After |
|--------|-------|
| `<GridView ItemType="Product" SelectMethod="GetProducts">` | `<GridView SelectMethod="GetProducts">` |
| `<ListView TItem="CartItem" SelectMethod="GetCartItems">` | `<ListView SelectMethod="GetCartItems">` |

---

## Build Verification

| Metric | Value |
|--------|-------|
| **Final Result** | ✅ Build succeeded |
| **Errors** | 0 |
| **Warnings** | 0 |
| **Build Attempts** | 4 (up from 3 in Run 10) |

### Build Issues Resolved During Iteration

| Attempt | Errors | Fix Applied |
|---------|--------|-------------|
| 1 | 1 (NETSDK1004) | `dotnet restore` needed |
| 2 | 4 | Escaped quotes in interpolated strings (`AdminPage.razor.cs`) |
| 3 | 7 | Missing `using` directives in code-behinds (`CartStateService`, `CartItem`, etc.) |
| 4 | 22 | Event handler stubs, enum qualifications (`LogoutAction`, `BorderStyle`, `WebColor`), ViewSwitcher, `#` color prefix, TemplateField ItemType |
| 5 | **0** | ✅ Build succeeded |

!!! tip "Build Attempt Analysis"
    The increase from 3 → 4 attempts is a minor regression caused by the **increased scope** of converted pages. AdminPage now has full EF Core CRUD (was a stub), and the three P0 recovery pages (CheckoutReview, ShoppingCart, ManageLogins) all carry more complex markup. The error categories (missing usings, enum qualifications, event handler stubs) are new Cycle 3 automation candidates.

---

## Remaining Gaps

| Area | Status | Notes |
|------|--------|-------|
| **Account/Identity pages** (15 pages) | ⚠️ Minimal/Stub | BWFC markup fully preserved; needs Identity scaffolding (Cycle 3) |
| **Checkout flow** (5 pages) | ⚠️ Stub | PayPal integration, order processing — needs payment service |
| **ViewSwitcher** | ❌ Stub | Mobile/desktop switching — not applicable to Blazor responsive design |
| **Mobile layout** | ❌ Stub | `Site.MobileLayout.razor` converted but not functional |
| **Cart persistence** | ⚠️ In-memory | `CartStateService` uses scoped in-memory list — not DB-backed |
| **Missing `using` directives** | ⚠️ Layer 1 gap | Code-behinds need auto-generation of common `using` statements |
| **Event handler stubs** | ⚠️ Layer 1 gap | Complex event handlers (e.g., admin CRUD) still need Layer 2 |

---

## Recommendations for Cycle 3

### Primary Target: Identity Scaffolding

Login and Register pages have **complete BWFC markup** — TextBox, Button, RequiredFieldValidator, Label — all correctly converted. The remaining gap is ASP.NET Identity integration:

1. **Scaffold ASP.NET Identity** into the Blazor project
2. **Wire Login/Register code-behinds** to Identity `SignInManager`/`UserManager`
3. **Validate ManageLogins** with real `UserLoginInfo` data (currently uses stub model)

### P3 Fixes (Target: Reduce Build Attempts from 4 → 1)

1. **Auto-generate `using` directives** — Layer 1 should emit common `using` statements (`CartStateService`, `CartItem`, model namespaces) in code-behinds based on referenced types.

2. **Event handler stub generation** — Complex handlers (admin CRUD, cart operations) still require Layer 2 manual work. Layer 1 could generate compilable no-op stubs with `// TODO` markers.

3. **Enum qualification expansion** — Extend `Convert-EnumAttributes` to cover `LogoutAction`, `BorderStyle`, `WebColor`, and `#` color prefix conversion.

### Stretch Goals

4. **Single-pass build** — If P3 fixes land, the goal is a **1 build attempt** pipeline: Layer 1 output compiles on first pass with zero Layer 2 fixes needed for compilation.

5. **Identity-aware stub models** — Generate stub models that implement the correct Identity interfaces, so markup can bind to real types even before full Identity scaffolding.

---

## Appendix: Full Transform Log Stats

### Layer 1 Output Summary

| Metric | Value |
|--------|-------|
| Files processed | 32 |
| Transforms applied | 354 |
| Static files copied | 79 |
| Manual review items | 46 |
| `-SkipProjectScaffold` | Yes |

### Output File Inventory

| Category | Count |
|----------|-------|
| `.razor` files | 32 |
| `.razor.cs` code-behinds | 32 |
| `wwwroot/` static files | 79 |
| Routable pages (`@page`) | 28 |
| BWFC control instances | **178** |
| Unique BWFC control types | **26** |

### Cycle 2 Script Fix Validation Matrix

| Fix | Layer 1 Function | Validated | Impact |
|-----|-------------------|-----------|--------|
| Enum attributes | `Convert-EnumAttributes` | ✅ | TextMode, Display, GridLines auto-converted |
| Boolean attributes | `Convert-BooleanAttributes` | ✅ | PascalCase → lowercase for all boolean attrs |
| ControlToValidate stripping | `Remove-ControlToValidate` | ✅ | Eliminates ~20 manual cleanup items per run |
| ImageButton detection | Validation pass elevation | ✅ | WARNING → FAIL prevents silent flattening |
| Expression wrapping | `Wrap-ServerExpressions` | ✅ | `<% %>` → `@* TODO *@` comment blocks |
| ItemType/TItem stripping | `Remove-ItemTypeWithDataSource` | ✅ | Eliminates #1 recurring build failure class |

---

*Report generated by Beast (Technical Writer) for the BWFC Migration Toolkit project.*
*Source data: `samples/Run11WingtipToys/BENCHMARK-DATA.md`*
*Improvement Loop: Cycle 2 of 3*
