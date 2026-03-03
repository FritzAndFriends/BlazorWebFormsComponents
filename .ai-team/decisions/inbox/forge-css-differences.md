# Decision: WingtipToys CSS Fidelity — Root Causes and Required Fixes

**Author:** Forge (Lead / Web Forms Reviewer)
**Date:** 2026-03-02
**Status:** Proposed
**Requested by:** Jeff Fritz

## Context

Side-by-side comparison of original WingtipToys (IIS Express :5200) vs. migrated Blazor (Kestrel :5201) revealed 7 visual/CSS differences. Jeff's manager will see these screenshots. All must be addressed.

## Findings

### CRITICAL — Fixes Required

#### 1. Wrong Bootstrap Theme (Navbar + All Colors)

The original WingtipToys ships a **Bootswatch "Cerulean" v3.2.0** theme as `Content/bootstrap.css`. The migrated `App.razor` loads **stock Bootstrap 3.4.1** from CDN instead.

| Property | Original (Cerulean) | Migrated (Stock BS3) |
|---|---|---|
| `.navbar-inverse` bg | `#033c73` (blue) | `#222222` (dark gray) |
| Nav link color | `#ffffff` (white) | `#999999` (gray) |
| `h1-h6` color | `#317eac` (teal-blue) | `#333333` (dark gray) |
| `a` color | `#2fa4e7` (sky blue) | `#337ab7` (std blue) |
| `a:hover` color | `#157ab5` | `#23527c` |

**Fix:** In `App.razor`, replace the two CDN `<link>` tags with:
```html
<link rel="stylesheet" href="/Content/bootstrap.css" />
<link rel="stylesheet" href="/Content/Site.css" />
```
The Bootswatch Cerulean file already exists at `samples/AfterWingtipToys/Content/bootstrap.css`. These files must be served from `wwwroot/` or a static file mapping must be configured.

#### 2. Product Grid — Single Column Instead of 4-Column

`ProductList.razor` is missing `GroupItemCount="4"`, `GroupTemplate`, and `LayoutTemplate`. The BWFC ListView fully supports these. The migrated file needs these templates added back.

**Fix:** Add to `ProductList.razor`:
```razor
<ListView ID="productList" DataKeyNames="ProductID" GroupItemCount="4"
    TItem="Product" Items="@Products">
    ...
    <GroupTemplate>
        <tr>
            <td id="itemPlaceholder">@context</td>
        </tr>
    </GroupTemplate>
    <LayoutTemplate>
        <table style="width:100%;">
            <tbody>
                <tr><td>
                    <table style="width:100%">@context</table>
                </td></tr>
            </tbody>
        </table>
    </LayoutTemplate>
    ...
</ListView>
```

### MODERATE — Should Fix

#### 3. Missing "Trucks" Category

`MainLayout.razor` hardcodes 4 categories; original has 5 (including "Trucks"). Either add the missing category or make it data-driven.

#### 4. Site.css Not Referenced

`App.razor` never loads `Content/Site.css`. This omits `body { padding-top: 50px }` and `.body-content` responsive padding. Fix is included in item #1 above.

#### 5. BoundField DataFormatString Bug (BWFC Core Bug)

`src/BlazorWebFormsComponents/BoundField.razor.cs` line 48:
```csharp
// BUG: converts to string before formatting, losing numeric format support
return RenderString(string.Format(DataFormatString, obj?.ToString()));
// FIX:
return RenderString(string.Format(DataFormatString, obj));
```
This causes `{0:c}` to render "15.95" instead of "$15.95" in the cart. This is a library-level bug affecting ALL DataFormatString consumers, not just WingtipToys.

### LOW — Cosmetic

#### 6. bootstrap-theme.min.css Loaded by Migrated Only

The CDN reference in `App.razor` also loads `bootstrap-theme.min.css` which adds gradient overlays to buttons and navbars. The original Bootswatch Cerulean does not include these gradients. Removing the CDN refs (fix #1) also fixes this.

#### 7. Cart Price Column Missing "$" Sign

This is a symptom of bug #5 above (BoundField DataFormatString). Fixing the core bug resolves this.

## Decision Needed

1. **CSS fix approach**: Should we (a) copy Bootswatch Cerulean CSS into `wwwroot/css/` and reference it, or (b) move the existing `Content/` folder under `wwwroot/` and update `App.razor`?
2. **BoundField bug**: Should this be filed as a separate issue for the BWFC library? It affects all consumers, not just WingtipToys.
3. **Category menu**: Should `MainLayout.razor` be made data-driven like the original, or just add "Trucks" as a hardcoded entry?

## Impact

Without these fixes, the migration showcase screenshots show a fundamentally different-looking app — dark gray vs. blue navbar, single-column vs. grid products, wrong heading/link colors throughout. This undermines the "same visual output" promise of BWFC.
