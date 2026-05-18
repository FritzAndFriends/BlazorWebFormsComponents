# WingtipToys Migration Run 31

## Summary

Run 31 validated the **compiled page imports fix**. The generated scaffold now writes BWFC global usings into `GlobalUsings.cs`, which means emitted page `.razor.cs` files no longer depend on NuGet-only package targets to resolve BWFC component types.

That fix worked for the original target bucket: the deduplicated `CS0246` failures dropped from **41 in run30 to 7 in run31**. The remaining `CS0246` errors are no longer the broad page-control import problem; they are now limited to deeper support-code issues such as `HttpException` and copied identity/application types in `Logic\RoleActions.cs`.

## Migration Result

- Files processed: **32**
- Files written: **176**
- Compile-surface artifacts quarantined: **6**
- App_Start files quarantined: **4**

## Build Result

- Build target: `samples\AfterWingtipToys\WingtipToys.csproj`
- Result: **failed**
- Reported build totals: **205 errors**, **112 warnings**

## What Improved

1. `GlobalUsings.cs` now includes:
   - `global using BlazorWebFormsComponents;`
   - `global using BlazorWebFormsComponents.Enums;`
   - `global using BlazorWebFormsComponents.LoginControls;`
   - `global using BlazorWebFormsComponents.Validations;`
2. Emitted page code-behind no longer fails en masse on missing `Button`, `Label`, `TextBox`, `GridView<T>`, `FormView<T>`, `RequiredFieldValidator<T>`, and similar BWFC types.
3. The prior “compiled page imports” bucket is now effectively cleared, exposing the next true blockers.

## New Dominant Failure Classes

Deduplicated run31 errors are now dominated by:

1. **Missing members / semantic migration gaps (`CS0103`)** — 71 errors  
   Generated markup and code-behind still disagree on members/events in pages like `CheckoutReview`, `CheckoutComplete`, `ShoppingCart`, `Site`, and account flows.

2. **Analyzer/style noise promoted to errors (`IDE0007`)** — 70 errors  
   Copied support files such as `Logic\PayPalFunctions.cs` now enter the build and fail on style analyzers, which should not be a migration blocker.

3. **API/runtime surface mismatches (`CS1061`, `CS0234`, `CS1929`, etc.)**  
   These now stand out more clearly in copied support code and identity-related logic.

4. **Validator inference and markup cleanup remain**  
   The existing `RZ10001`, `RZ9980`, `RZ9981`, and `RZ9996` buckets are still present and unchanged from run30.

## Conclusion

Run 31 confirms the imports fix should be kept. It solved the specific page-codebehind namespace problem and moved the benchmark forward to the next layers:

1. semantic member alignment between generated markup and emitted code-behind,
2. compile-surface filtering or warning suppression for copied support code,
3. validator/template/server-block markup cleanup.
