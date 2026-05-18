# WingtipToys Toolkit Migration Rerun

## Summary

This rerun cleared `samples\AfterWingtipToys`, migrated `samples\WingtipToys` back into that folder using `migration-toolkit\scripts\bwfc-migrate.ps1`, then restored and built the generated project to assess the fresh output.

- Migration time: `00:00:02.4024826`
- Source files transformed: `32`
- Mechanical transforms applied: `615`
- Static files copied: `156`
- Manual-review items flagged by the toolkit: `51`
- Restore result: succeeded
- Build result: failed with `114` errors and `33` warnings

## What worked well

1. **Fast mechanical conversion** — the toolkit regenerated the output project in about 2.4 seconds.
2. **Project scaffold generation** — `WingtipToys.csproj`, `Program.cs`, `_Imports.razor`, `GlobalUsings.cs`, and component/layout files were recreated successfully.
3. **Static asset carry-forward** — 156 static files were copied into the new app.
4. **Manual-attention inventory** — the toolkit surfaced 51 follow-up items, including route helpers, select methods, redirect handlers, session state, and leftover code blocks.
5. **Restore path is healthy** — the generated project restored successfully against the repo-local BWFC projects.

## What did not work well

1. **The migration-toolkit path did not pick up the new CLI namespace/import fix.**
   - Fresh `_Imports.razor` still contains:
     - `@using WingtipToys`
     - `@using WingtipToys.Models`
   - It does **not** contain:
     - `@namespace WingtipToys`
     - `@using global::WingtipToys`
     - `@using global::WingtipToys.Models`
   - Example generated code-behind still uses `namespace WingtipToys.Account` even though the file lives under `WingtipToys\Account\Login.razor.cs`.

2. **Namespace/import mismatches are still a primary compile blocker in this toolkit flow.**
   - Build output includes repeated errors such as:
     - `_Imports.razor(11,20): error CS0234: The type or namespace name 'Models' does not exist in the namespace 'WingtipToys.WingtipToys'`
   - This is the same nested-folder namespace problem the CLI fix was intended to address, which indicates the PowerShell toolkit script is still using older scaffold/transform behavior.

3. **Malformed Razor remains in several migrated pages.**
   - `Account\Manage.razor`: unclosed `%` tags (`RZ9980`)
   - `Checkout\CheckoutReview.razor`: invalid `TemplateField` child content (`RZ9996`)
   - `ProductList.razor`: unclosed `<b>` tags and invalid `ListView` child content
   - `ShoppingCart.razor`: unclosed `%#:` block

4. **Validation/import gaps remain in the scaffolded project.**
   - Many pages report `RZ10012` for components like `RequiredFieldValidator`, `CompareValidator`, `ModelErrorMessage`, and `RegularExpressionValidator`, which points to missing BWFC validation imports in the generated project-level `_Imports.razor`.

5. **Source-file copy coverage is still incomplete for a clean compile.**
   - The build reports many `CS0234` errors for missing `WingtipToys.Models` and `WingtipToys.Logic` namespaces from generated pages and layout code.
   - The migration summary also reported `Model files copied: 0` and `BLL files copied: 0`.

## Key evidence from this rerun

- Timing: `docs\migration-tests\wingtiptoys-toolkit-2026-04-27-rerun\timing.txt`
- Restore output: `docs\migration-tests\wingtiptoys-toolkit-2026-04-27-rerun\restore-output.md`
- Build output: `docs\migration-tests\wingtiptoys-toolkit-2026-04-27-rerun\build-output.md`
- Fresh generated imports: `samples\AfterWingtipToys\_Imports.razor`
- Fresh generated code-behind sample: `samples\AfterWingtipToys\WingtipToys\Account\Login.razor.cs`

## Conclusion

The toolkit rerun was successful as a **Layer 1 mechanical migration benchmark**: it regenerated the project quickly, copied assets, and surfaced a useful manual-work inventory.

It was **not** successful as a compile-ready migration. The biggest result from this fresh run is that the **migration-toolkit PowerShell path is still bypassing the new CLI namespace/import alignment behavior**, so the namespace problem remains reproducible when the toolkit script is used directly.
