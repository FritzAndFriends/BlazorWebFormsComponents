# Bishop decision — fix BWFC data-control template emission

**Date:** 2026-05-07T13:17:32-04:00  
**By:** Bishop  
**Requested by:** Jeffrey T. Fritz

## Decision
Add a dedicated CLI markup transform to propagate typed `GridView ItemType` values down into child BWFC column components, and extend template-context normalization so `ListView` group/layout placeholders emit explicit fragment contexts.

## What changed
1. Added `GridViewColumnItemTypeTransform` at Order 705 after `AttributeStripTransform`.
2. Rewrote typed `GridView` child `BoundField`, `TemplateField`, `HyperLinkField`, and `ButtonField` tags from `ItemType="object"` to the parent grid item type.
3. Extended `TemplateContextTransform` so `GroupTemplate` emits `Context="items"` and `LayoutTemplate` emits `Context="groups"` when no explicit context exists.
4. Added CLI tests covering Wingtip-style `ShoppingCart` and `ProductList` emission.
5. Updated CLI docs to document the new transform behavior.

## Why
Run 40 showed Layer 1 output still needed manual structural cleanup on flagship BWFC data controls. The worst failure mode was typed `GridView` templates compiling against `object`, which makes generated expressions like `@Item.Quantity` invalid. Explicit ListView placeholder contexts also make the generated template structure trustworthy and easier to inspect.

## Validation
- `dotnet test tests\BlazorWebFormsComponents.Cli.Tests --nologo`
- `dotnet run --project src\BlazorWebFormsComponents.Cli -- convert -i samples\WingtipToys\WingtipToys\ProductList.aspx -o .\bishop-output --overwrite`
- `dotnet run --project src\BlazorWebFormsComponents.Cli -- convert -i samples\WingtipToys\WingtipToys\ProductDetails.aspx -o .\bishop-output --overwrite`
- `dotnet run --project src\BlazorWebFormsComponents.Cli -- convert -i samples\WingtipToys\WingtipToys\ShoppingCart.aspx -o .\bishop-output --overwrite`

## Consequences
Layer 1 now emits cleaner BWFC ListView placeholders and correctly typed GridView columns for Wingtip-style pages, reducing manual repair on the benchmark path. FormView item templates continue to emit valid typed fragments, and the CLI suite now guards this contract.
