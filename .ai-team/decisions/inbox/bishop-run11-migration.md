# Bishop: Run 11 Migration Results & Decisions

**Author:** Bishop (Migration Tooling Dev)
**Date:** 2025-07-25

## Summary

Run 11 WingtipToys migration completed with all 3 P0 gaps from Run 10 closed. Build succeeded — 0 errors, 0 warnings, 4 build attempts. 178 BWFC control instances across 26 unique types.

## Decisions Made

### D1: ItemType/TItem Stripping When SelectMethod Present

**Context:** Jeff directed that ItemType/TItem should NOT be emitted when a data source (SelectMethod) is present. Blazor generic type inference handles it automatically. This was the #1 recurring build failure class across Runs 9 and 10.

**Decision:** Added `Remove-ItemTypeWithDataSource` function to bwfc-migrate.ps1. Runs before `ConvertFrom-SelectMethod` so it can detect both attributes on the same tag. Strips ItemType or TItem when SelectMethod is co-present. Tags without SelectMethod keep ItemType as-is.

**Impact:** Eliminates redundant type parameter errors. Affects GridView, ListView, FormView, DetailsView, DropDownList and any other control with SelectMethod+ItemType combination.

### D2: Stub Model Pattern for Missing Types

**Context:** ManageLogins uses `Microsoft.AspNet.Identity.UserLoginInfo` and CheckoutReview uses shipping info types that don't exist in the Blazor project. Previous runs either deleted BWFC markup (stub page) or left compile errors.

**Decision:** Create lightweight stub model classes in the project's Models/ directory:
- `UserLoginInfo` (LoginProvider, ProviderKey) — replaces Microsoft.AspNet.Identity.UserLoginInfo
- `OrderShipInfo` (FirstName, LastName, Address, City, State, PostalCode, Total) — supports DetailsView binding

**Rationale:** This preserves ALL BWFC markup while making the project compile. The stub models can be replaced with real implementations when identity/payment systems are integrated.

### D3: DetailsView Data Binding with Placeholder List

**Context:** The CheckoutReview DetailsView needs to bind to shipping address data that would come from a payment processor (PayPal) in the original app.

**Decision:** Bind DetailsView with `Items="@_shipInfoList"` where `_shipInfoList` contains a single `OrderShipInfo` with placeholder data populated in OnInitialized. This preserves the full DetailsView+TemplateField+Label structure while making the page functional.

### D4: ImageButton Preserved with OnClick Navigation

**Context:** ShoppingCart's checkout button was originally an `<asp:ImageButton>` that posted to PayPal. In Run 10, it was flattened to `<img>`. BWFC has an ImageButton component.

**Decision:** Preserve as `<ImageButton>` with `OnClick="@OnCheckout"` that calls `Nav.NavigateTo("/CheckoutReview")`. The PayPal-specific behavior is replaced with in-app navigation, but the BWFC component and its visual appearance are preserved.

## Remaining Issues

1. **bwfc-scan.ps1 parse error** — Line 365 area. Needs investigation (brace/syntax issue). Layer 0 scan could not run.
2. **Layer 1 enum conversion gaps** — `LogoutAction="Redirect"`, `BorderStyle="None"`, `BackColor="Transparent"` not converted to `@LogoutAction.Redirect`, `@BorderStyle.None`, `@WebColor.Transparent`. These require Layer 2 manual fixes. Consider adding to `Convert-EnumAttributes`.
3. **`#hexcolor` preprocessor ambiguity** — `BorderColor="#efeeef"` is parsed as preprocessor directive by Razor. Needs `@("#efeeef")` escaping. Layer 1 could detect and escape these.

## Metrics

| Metric | Run 10 | Run 11 | Delta |
|--------|--------|--------|-------|
| Build attempts | 3 | 4 | +1 |
| BWFC instances | 172 | 178 | +6 |
| P0 issues | 3 | 0 | -3 ✅ |
| Preservation rate | 92.7% | ~100% | +7.3% |
