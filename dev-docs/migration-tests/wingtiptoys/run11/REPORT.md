# WingtipToys Migration — Run 11 Report

## Status: ❌ FAILED — 17/25 tests passed, 8 failures

## Executive Summary

Run 11 attempted a fully automated migration from scratch following Jeff's prescribed order:
fresh Blazor project → BWFC library → migration script → static content → C# adaptation → acceptance tests.

The migration agent (Cyclops) completed the build phase with 0 errors and 105 files generated.
However, **8 of 25 acceptance tests failed**, revealing systemic gaps in the migration toolchain
that no amount of Layer 2 manual work can paper over — these are tooling defects that must be
fixed upstream.

**Result: 17 passed, 8 failed (68% pass rate)**

---

## Test Results

| Test | Result | Root Cause |
|------|--------|------------|
| NavigationTests.HomePage_Loads | ✅ PASS | — |
| NavigationTests.NavbarLink_LoadsPage("About") | ✅ PASS | — |
| NavigationTests.NavbarLink_LoadsPage("Contact") | ✅ PASS | — |
| NavigationTests.NavbarLink_LoadsPage("ProductList") | ✅ PASS | — |
| NavigationTests.ShoppingCartLink_LoadsPage | ✅ PASS | — |
| NavigationTests.LoginLink_LoadsPage | ✅ PASS | — |
| NavigationTests.RegisterLink_LoadsPage | ✅ PASS | — |
| AuthenticationTests.LoginPage_HasExpectedFormFields | ✅ PASS | — |
| AuthenticationTests.RegisterPage_HasExpectedFormFields | ✅ PASS | — |
| AuthenticationTests.RegisterAndLogin_EndToEnd | ❌ FAIL | RC-11 |
| StaticAssetTests.CssFiles_ReturnHttp200 | ✅ PASS | — |
| StaticAssetTests.HomePage_LoadsAtLeastOneCssFile | ✅ PASS | — |
| StaticAssetTests.Navbar_HasBootstrapClasses | ✅ PASS | — |
| StaticAssetTests.Navbar_HasReasonableHeight | ✅ PASS | — |
| StaticAssetTests.HomePage_Screenshot_VerifyLayout | ✅ PASS | — |
| StaticAssetTests.HomePage_HasStyledMainContent | ❌ FAIL | RC-8 |
| StaticAssetTests.HomePage_NoFailed_StaticAssetRequests | ❌ FAIL | RC-7 |
| StaticAssetTests.ProductList_ImageRequests_ReturnHttp200 | ✅ PASS | — |
| StaticAssetTests.ProductList_AllImagesLoad | ✅ PASS | — |
| StaticAssetTests.ProductList_Screenshot_VerifyImagesAndLayout | ✅ PASS | — |
| StaticAssetTests.ProductDetails_Screenshot_VerifyImageAndStyling | ❌ FAIL | RC-9 |
| ShoppingCartTests.ProductList_DisplaysProducts | ❌ FAIL | RC-9 |
| ShoppingCartTests.AddItemToCart_AppearsInCart | ❌ FAIL | RC-9 |
| ShoppingCartTests.UpdateCartQuantity_ChangesItemCount | ❌ FAIL | RC-9 |
| ShoppingCartTests.RemoveItemFromCart_EmptiesCart | ❌ FAIL | RC-9 |

---

## Root Cause Analysis

### RC-7: Scripts/ folder not copied to wwwroot (SCRIPT DEFECT)
**Severity:** High
**Component:** `bwfc-migrate.ps1` static asset copy + migration-standards SKILL

The migration script's `Invoke-CssAutoDetection` function detects CSS files in `Content/` and
adds `<link>` tags. However, **it does not detect or copy JavaScript files from `Scripts/`**.
The original Web Forms app uses `<webopt:bundlereference>` to reference bundled JS (jQuery,
Bootstrap). The script correctly converts CSS bundle references but completely ignores JS bundles.

The migration-standards SKILL lists CSS verification but not JS asset verification. The Cyclops
agent instructions listed `Content/`, `Images/`, `Catalog/`, `fonts/`, and `favicon.ico` to
copy — but not `Scripts/`.

**404 errors observed:**
- `/Scripts/jquery-1.10.2.min.js`
- `/Scripts/bootstrap.min.js`

**Fix needed in:** `bwfc-migrate.ps1`, `migration-standards/SKILL.md`

---

### RC-8: Homepage has no visible styled content (CONTENT GAP)
**Severity:** Medium
**Component:** Migration agent Layer 2 completeness

The Default.razor homepage is minimal — just a heading and paragraph. The original Default.aspx
had more content including a product showcase area. The acceptance test checks that the main
content area has a reasonable height (>100px). With just a heading and paragraph, the content
area is only ~50px tall.

This is a completeness gap in the Layer 2 migration work, not a tooling defect.

---

### RC-9: ListView renders empty — LayoutTemplate/GroupTemplate placeholder not converted (SCRIPT DEFECT + BWFC GAP)
**Severity:** ⚠️ CRITICAL
**Component:** `bwfc-migrate.ps1` template conversion + BWFC ListView documentation

This is the **most impactful defect** in Run 11, causing 5 of the 8 failures.

The ProductList page uses a `<ListView>` with `GroupItemCount="4"`, `LayoutTemplate`, and
`GroupTemplate`. The script correctly converts ASP.NET Web Forms markup to BWFC Razor syntax,
but **does not convert placeholder elements to `@context` references**.

**What the script produces (broken):**
```razor
<LayoutTemplate>
    <table style="width:100%;">
        <tbody><tr><td>
            <table id="groupPlaceholderContainer" style="width:100%">
                <tr id="groupPlaceholder"></tr>  <!-- STATIC HTML — does nothing -->
            </table>
        </td></tr></tbody>
    </table>
</LayoutTemplate>
<GroupTemplate>
    <tr id="itemPlaceholderContainer">
        <td id="itemPlaceholder"></td>  <!-- STATIC HTML — does nothing -->
    </tr>
</GroupTemplate>
```

**What it should produce (working):**
```razor
<LayoutTemplate>
    <table style="width:100%;">
        <tbody><tr><td>
            <table id="groupPlaceholderContainer" style="width:100%">
                @context
            </table>
        </td></tr></tbody>
    </table>
</LayoutTemplate>
<GroupTemplate>
    <tr id="itemPlaceholderContainer">
        @context
    </tr>
</GroupTemplate>
```

**Why this happens:** In ASP.NET Web Forms, `<tr id="groupPlaceholder">` is a **runtime
placeholder** — the server control replaces it with actual rendered content at runtime. In BWFC
Blazor, `LayoutTemplate` and `GroupTemplate` are `RenderFragment<RenderFragment>` parameters —
the `@context` parameter IS the child content. Without rendering `@context`, the items are
computed internally but never placed in the DOM.

**Impact:** ALL data controls using LayoutTemplate/GroupTemplate (ListView, DataPager) will
produce empty output after migration. This affects every data-bound page in every migration.

**Fix needed in:** `bwfc-migrate.ps1` (placeholder→@context conversion), BWFC documentation

---

### RC-10: ProductDetails page lacks "Add to Cart" link (LAYER 2 GAP)
**Severity:** Medium
**Component:** Migration agent Layer 2 work

The migrated ProductDetails.razor shows product info but **has no "Add to Cart" link**. The
original ProductDetails.aspx included an action link to add the product to the shopping cart.
Shopping cart tests navigate to ProductDetails and look for `a[href*='AddToCart']` — they time
out after 30 seconds when this link doesn't exist.

---

### RC-11: Register/Login flow doesn't produce authenticated state (AUTH GAP)
**Severity:** Medium
**Component:** Auth cookie flow with InteractiveServer + HTTP POST endpoints

The Register and Login pages correctly use HTML forms posting to HTTP endpoints (required
pattern since SignInManager needs HTTP context, not a SignalR circuit). However, after the
register→login flow, the page doesn't display authenticated state markers ("Hello", "Manage",
"Log out", or the user's email).

Likely cause: After the HTTP POST login endpoint sets the auth cookie and redirects to `/`,
the Blazor circuit may not pick up the new authentication state. The `CascadingAuthenticationState`
service needs to be notified of the state change, or the redirect needs to force a full page
reload (not a Blazor-enhanced navigation).

---

## Recommended Fixes to Scripts, Skills, and BWFC

### Fix 1: bwfc-migrate.ps1 — Add Scripts/ folder detection and copy
**File:** `migration-toolkit/scripts/bwfc-migrate.ps1`
**Priority:** High
**Effort:** Small

Add a `Invoke-ScriptAutoDetection` function (parallel to `Invoke-CssAutoDetection`) that:
1. Scans the source project for a `Scripts/` folder
2. Copies all JS files to `wwwroot/Scripts/` in the output
3. Adds `<script>` tags to App.razor for detected JS files (jquery, bootstrap at minimum)
4. Handles `<webopt:bundlereference Runat="server" Path="~/Scripts/js">` conversion

---

### Fix 2: bwfc-migrate.ps1 — Convert ListView/DataPager placeholder elements to @context
**File:** `migration-toolkit/scripts/bwfc-migrate.ps1`
**Priority:** ⚠️ CRITICAL
**Effort:** Medium

Add a post-processing step that converts placeholder patterns inside templates to `@context`.

Specific patterns to match and replace:
```
# Inside LayoutTemplate:
<tr id="groupPlaceholder"></tr>              → @context
<tr id="groupPlaceholder" />                 → @context

# Inside GroupTemplate:
<td id="itemPlaceholder"></td>               → @context
<td id="itemPlaceholder" />                  → @context

# General pattern for any template:
<element id="*[Pp]laceholder*">...</element>  → @context
```

Recommended implementation approach:
```powershell
# After ASPX→Razor conversion, scan for placeholder elements inside *Template blocks
# Replace the placeholder element (and its children) with @context
$placeholderPattern = '<(tr|td|div|span)\s+id="[^"]*[Pp]laceholder[^"]*"\s*/?\s*>.*?</\1>'
$content = [regex]::Replace($content, $placeholderPattern, '@context', 'Singleline')
# Also handle self-closing: <tr id="groupPlaceholder" />
$selfClosingPattern = '<(tr|td|div|span)\s+id="[^"]*[Pp]laceholder[^"]*"\s*/>'
$content = [regex]::Replace($content, $selfClosingPattern, '@context')
```

---

### Fix 3: migration-standards SKILL — Add static asset completeness checklist
**File:** `.ai-team/skills/migration-standards/SKILL.md`
**Priority:** High
**Effort:** Small

Add a "Static Asset Migration Checklist" section that explicitly lists ALL common folders:

```markdown
## Static Asset Migration Checklist
Copy ALL of these from the source Web Forms project to `wwwroot/` in the Blazor project:
- `Content/` → `wwwroot/Content/` (CSS files)
- `Scripts/` → `wwwroot/Scripts/` (JavaScript — jQuery, Bootstrap, app scripts)
- `Images/` → `wwwroot/Images/` (site images, logos)
- `Catalog/` → `wwwroot/Catalog/` (product/content images)
- `fonts/` → `wwwroot/fonts/` (web fonts)
- `favicon.ico` → `wwwroot/favicon.ico`

Then verify:
- [ ] All `<link>` CSS references in App.razor have matching physical files
- [ ] All `<script>` JS references in App.razor have matching physical files
- [ ] All `<img>` src paths in pages have matching physical files
```

---

### Fix 4: migration-standards SKILL — Add ListView template migration guide
**File:** `.ai-team/skills/migration-standards/SKILL.md`
**Priority:** ⚠️ CRITICAL
**Effort:** Small

Add a new section explaining how Web Forms template placeholders map to Blazor:

```markdown
## ListView LayoutTemplate/GroupTemplate Placeholder Conversion

In ASP.NET Web Forms, `<tr id="groupPlaceholder">` inside LayoutTemplate is a runtime
placeholder that the server control replaces with rendered group content.

In BWFC Blazor, templates are `RenderFragment<RenderFragment>` — use `@context` to render
child content where the placeholder element was.

### Before (Web Forms):
<LayoutTemplate>
    <table><tr id="groupPlaceholder"></tr></table>
</LayoutTemplate>
<GroupTemplate>
    <tr><td id="itemPlaceholder"></td></tr>
</GroupTemplate>

### After (BWFC Blazor):
<LayoutTemplate>
    <table>@context</table>
</LayoutTemplate>
<GroupTemplate>
    <tr>@context</tr>
</GroupTemplate>

This applies to ALL data controls that use placeholder-based templates.
```

---

### Fix 5: migration-standards SKILL — Add auth flow pattern for HTTP POST login
**File:** `.ai-team/skills/migration-standards/SKILL.md`
**Priority:** Medium
**Effort:** Small

Document the auth cookie flow issue:

```markdown
## Authentication: HTTP POST Login with InteractiveServer

SignInManager requires an HTTP context — it cannot work inside a SignalR circuit.
Use HTML forms that POST to mapped HTTP endpoints.

IMPORTANT: After login/register, the redirect MUST force a full page reload (not
Blazor-enhanced navigation) so the auth cookie is picked up by the circuit.

Use `data-enhance="false"` on the `<form>` element AND ensure the HTTP endpoint
returns `Results.Redirect("/")` — this triggers a full navigation, not a Blazor patch.
```

---

### Fix 6: BWFC Library — Consider runtime warning for empty placeholders
**File:** `src/BlazorWebFormsComponents/ListView.razor`
**Priority:** Low (nice-to-have)
**Effort:** Medium

Consider making the ListView component detect when a LayoutTemplate or GroupTemplate renders
static placeholder-like HTML but `@context` is never invoked. The component could log a
development-time warning: "LayoutTemplate does not render @context — items will not be visible."

This would make post-migration debugging much easier for developers who migrate from Web Forms
and don't realize placeholders need manual conversion.

---

## Run History

| Run | Date | Result | Pass Rate | Key Issue |
|-----|------|--------|-----------|-----------|
| 8 | 2026-03-06 | ✅ PASS | 25/25 | Visual baseline established |
| 9 | 2026-03-06 | ❌ FAIL | ~15/25 | No CSS/images — static assets not copied |
| 10 | 2026-03-07 | ❌ FAIL | N/A | Process violation — coordinator hand-edited files |
| 11 | 2026-03-07 | ❌ FAIL | 17/25 | ListView empty (placeholder→@context), Scripts/ missing |

## Conclusion

Run 11 demonstrates clear progress — navigation, CSS loading, image serving, and basic page
structure all work (17/25 pass). The remaining 8 failures trace to **two systemic tooling
defects** (RC-7 and RC-9) that will affect every migration, not just WingtipToys:

1. **The migration script doesn't convert template placeholders to `@context`** — this will
   break every ListView/DataPager with LayoutTemplate across all migrations.
2. **The migration script and SKILL don't handle JavaScript static assets** — any app with
   jQuery/Bootstrap JS will have broken interactivity.

Fixing these two critical issues in the script and SKILL should bring the next run to 22-23/25
passing, with only the auth flow and homepage content remaining as Layer 2 manual work items.
