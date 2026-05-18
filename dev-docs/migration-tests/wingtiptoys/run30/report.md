# WingtipToys Migration Run 30

## Summary

Run 30 validated the new **compiled page code-behind path** and the merged page runtime shim. The migration still does not build cleanly, but the generated WingtipToys app dropped from **183 build errors in run29 to 60 build errors in run30**.

The important behavior change is that the CLI no longer quarantines every page code-behind file. In this run it emitted **20 compiled `.razor.cs` files** and quarantined only **12 page/user-control/master-page code-behind artifacts** that still contain unsupported patterns.

## Migration Result

- Files processed: **32**
- Files written: **176**
- Static files copied: **80**
- Source files copied: **9**
- Compile-surface artifacts quarantined: **6**
- App_Start files quarantined: **4**

## Build Result

- Build target: `samples\AfterWingtipToys\WingtipToys.csproj`
- Result: **failed**
- Warnings: **23**
- Errors: **60**

## What Improved

1. Shim-compatible pages now emit real compiled code-behind files instead of always landing in `migration-artifacts\codebehind\`.
2. The merged `Page` / `WebFormsPageBase` runtime keeps head rendering on the shared page shim surface without forcing layout wrappers to duplicate page-service logic.
3. The fresh Wingtip run now compiles far enough that the dominant failures are narrower and more actionable than the run29 “missing page members everywhere” failure wall.

## Remaining Top Gaps

1. **Missing generated usings/type imports in emitted `.razor.cs` files**  
   Several compiled page partials now fail on BWFC control types such as `Button`, `Label`, `Panel`, `GridView<>`, `FormView<>`, and `RequiredFieldValidator<>`. The compiled-codebehind path needs companion using/import normalization.

2. **Legacy page-model attributes still leak into compiled code-behind**  
   `ProductDetails.razor.cs` still contains Web Forms model-binding attributes like `[QueryString]` and ambiguous `RouteData` references. Those need transform-time normalization before emission.

3. **Validator inference and templated markup normalization are still incomplete**  
   Account pages still fail with `RZ10001` for `RequiredFieldValidator`, and pages like `CheckoutReview.razor` / `ProductList.razor` still contain invalid templated markup shapes.

4. **Unresolved server-block markup still escapes into generated pages**  
   `Manage.razor`, `ProductList.razor`, and `ShoppingCart.razor` still contain `%`/`<%#:` fragments or malformed HTML. Those pages were correctly quarantined or left uncompiled, but the markup transforms still need stronger cleanup.

5. **Master-page shell still leaves unresolved script-manager style markup**  
   `Site.razor` now avoids compile-surface code-behind failure, but it still carries unresolved `Scripts` / `ScriptReference` markup that blocks the generated app build.

## Conclusion

Run 30 confirms that the new page-codebehind architecture is worth keeping: it materially reduces the Wingtip compile surface failure count and exposes the next concentrated gaps. The next follow-up should focus on **compiled code-behind import normalization** plus the existing **validator/template/server-block markup** cleanup work.
