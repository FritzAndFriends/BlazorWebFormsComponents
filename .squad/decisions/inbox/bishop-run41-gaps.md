# Decision: Run 41 quarantine, static-file, and antiforgery hardening

**Date:** 2026-05-07T15:38:16-04:00  
**Author:** Bishop  
**Requested by:** Jeffrey T. Fritz  
**Status:** Proposed

## Decision

1. Add an essential-page allowlist to `PageQuarantineDetector` so benchmark-critical product, catalog, cart, shopping, home, about, and contact paths are not quarantined for incidental heuristic signals.
2. Raise heuristic quarantine sensitivity so a single weak signal does not quarantine ordinary pages unless the path is clearly non-essential (`Account/`, `Admin/`, `Checkout/`, mobile shells) or the blocker is strong enough to break the compile surface.
3. Generate `Program.cs` with `app.UseStaticFiles();` and `app.UseAntiforgery();` for all scaffolded SSR apps.
4. Post-process semantic-pattern form output so generated `<form>`/`<EditForm>` markup receives `<AntiforgeryToken />` and a deterministic form name automatically.

## Rationale

Run 41 proved the CLI was over-quarantining core Wingtip pages, under-configuring the SSR scaffold for copied static assets, and leaving generated POST forms without the Blazor antiforgery contract. The chosen fix keeps benchmark paths runnable, preserves quarantine for true out-of-scope areas, and hardens generated SSR forms without duplicating form logic inside every semantic pattern.

## Files

- `src\BlazorWebFormsComponents.Cli\Pipeline\PageQuarantineDetector.cs`
- `src\BlazorWebFormsComponents.Cli\Scaffolding\ProgramCsEmitter.cs`
- `src\BlazorWebFormsComponents.Cli\SemanticPatterns\SemanticPatternCatalog.cs`
- `src\BlazorWebFormsComponents.Cli\Transforms\Markup\FormAntiforgeryPostProcessor.cs`
- `tests\BlazorWebFormsComponents.Cli.Tests\TransformUnit\PageQuarantineDetectorTests.cs`
- `tests\BlazorWebFormsComponents.Cli.Tests\ScaffoldingTests.cs`
- `tests\BlazorWebFormsComponents.Cli.Tests\SemanticPatternCatalogTests.cs`
- `tests\BlazorWebFormsComponents.Cli.Tests\SemanticPatternConcreteTests.cs`
- `tests\BlazorWebFormsComponents.Cli.Tests\PipelineIntegrationTests.cs`
- `docs\cli\index.md`
- `docs\cli\transforms.md`

## Validation

- `dotnet test tests\BlazorWebFormsComponents.Cli.Tests --nologo`
