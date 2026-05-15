# Decisions

> Shared team decisions. All agents read this. Only Scribe writes here (by merging from inbox).

# Bishop decision inbox — G3/G4 fixes

- **Date:** 2026-05-08T10:42:43-04:00
- **Owner:** Bishop

## Decision
For benchmark-facing identity scaffolds, the CLI should emit one consistent auth contract end to end:
- account semantic rewrites post to `/Account/LoginHandler` and `/Account/RegisterHandler`
- generated `Program.cs` configures application cookie `LoginPath` and `LogoutPath`
- redirect handler stubs use ASP.NET Core Identity (`SignInManager<IdentityUser>` / `UserManager<IdentityUser>`) and preserve `ReturnUrl` when it is local

For validator typing, `RequiredFieldValidator` should infer its generic `Type` from the validated control when possible (notably `TextBox` -> `string`) and only fall back to `object` when no control hint exists.

## Why
Run 42 showed that mismatched auth contracts caused the only first-pass failure, while blanket validator defaults created avoidable generic warnings. Encoding both decisions in the CLI keeps Layer 1 output runnable without forcing manual post-processing.


# Bishop G6/G7 Fixes

Date: 2026-05-08T11:37:18.862-04:00
Branch: feature/wingtip-next-features-review
PR: #545

## Decisions

1. `PageDirectiveTransform` now emits a source-relative primary `@page` route for nested `.aspx` pages and keeps the filename-only alias as a secondary route when the two differ. Root-level pages still emit a single route, and `Default.aspx`/`Index.aspx` continue to resolve to `/`.
2. `PageQuarantineDetector` now bypasses quarantine for redirect-only action pages when their markup is inert and their code-behind uses `Response.Redirect`, unless identity or payment signals are also present.
3. CLI docs were updated to describe nested route aliasing and the redirect-only quarantine exemption because both behaviors change generated migration output in operator-visible ways.
# Decision: ComponentRefCodeBehindTransform ClassOpenRegex Fix

**Date:** 2026-05-15T09:55:50-04:00  
**Author:** Bishop  
**Status:** Implemented

## Problem

`ComponentRefCodeBehindTransform` (Order 220) was silently skipping ALL field injection because its `ClassOpenRegex` (`partial\s+class\s+\w+\s*\{`) failed to match class declarations that include a base class.

`BaseClassStripTransform` (Order 200) runs **before** our transform and rewrites:
```
public partial class ShoppingCart : System.Web.UI.Page
```
to:
```
public partial class ShoppingCart : WebFormsPageBase
```

The regex `\s*\{` requires only whitespace before `{` — it cannot match `: WebFormsPageBase\n    {`. Result: every `@ref` field was never injected, producing 15+ CS0103 errors per page in Run 80.

## Decision

Change `ClassOpenRegex` from:
```csharp
@"partial\s+class\s+\w+\s*\{"
```
to:
```csharp
@"partial\s+class\s+\w+[^{]*\{"
```

`[^{]*` matches any characters (including `: WebFormsPageBase`, `, IDisposable`, newlines, spaces) up to the first `{`.

## Rationale

- Surgical one-character-class change with no side effects.
- `[^{]*` is the idiomatic pattern for "skip anything before the next `{`" in class-declaration regexes.
- Pattern generalizes to future transforms that need to locate the class body opening brace.

## Rule Established

> Any code-behind transform that locates the class body opening brace via regex **must** use `[^{]*\{` (not `\s*\{`) to account for base classes and interface lists injected by earlier transforms.

## Verification

- 35/35 `ComponentRef` tests pass (8 pre-existing + 3 new regression cases covering `: WebFormsPageBase`, `: Base, IInterface`, and brace-on-next-line variants).
- CLI build clean.


# Decision: Lock GridView TemplateField Preservation with Dual-Layer Regression Tests

**Date:** 2026-05-08T13:02:09-04:00
**Author:** Bishop
**Status:** Proposed

## Decision
Add TemplateField preservation regression coverage at both layers of the CLI test surface:

1. `tests/BlazorWebFormsComponents.Cli.Tests/TransformUnit/GridViewColumnItemTypeTransformTests.cs`
2. `tests/BlazorWebFormsComponents.Cli.Tests/PipelineIntegrationTests.cs`

The tests must prove that `TemplateField` columns survive alongside `BoundField` siblings and when they are the only GridView columns, while preserving nested `TextBox`, `CheckBox`, and inline display-expression content.

## Rationale
TemplateField loss is a high-severity migration failure because it silently drops editable inputs, calculated output, and row-selection controls. The current pipeline behavior depends on multiple ordered markup transforms working together, so a single focused unit test is not enough; we need both transform-level and end-to-end file-migration assertions to keep future ordering changes from regressing Wingtip-style GridViews.

## Files
- `tests/BlazorWebFormsComponents.Cli.Tests/TransformUnit/GridViewColumnItemTypeTransformTests.cs`
- `tests/BlazorWebFormsComponents.Cli.Tests/PipelineIntegrationTests.cs`


# Decision: ComponentRefCodeBehindTransform Test Coverage Complete

**Date:** 2026-05-15
**Author:** Rogue (QA Analyst)
**Status:** Informational — no action required from team

## Finding

The `ComponentRefCodeBehindTransformTests.cs` test file was already fully populated
with 10 well-structured tests covering all required scenarios. All 10 tests pass
against the current transform implementation.

## Test Coverage Map

| Scenario | Test Method | Status |
|---|---|---|
| Basic field generation (Label) | `InjectsFieldForLabel` | ✅ Pass |
| Basic field generation (generic) | `InjectsFieldForGenericGridView` | ✅ Pass |
| Multiple refs | `InjectsMultipleFields` | ✅ Pass |
| Field inserted before methods | `InsertsAfterClassOpeningBrace` | ✅ Pass |
| Empty ComponentRefs → unchanged | `SkipsWhenNoComponentRefs` | ✅ Pass |
| No class found → passthrough | `SkipsWhenNoClassFound` | ✅ Pass |
| No duplicate declarations | `SkipsExistingFieldDeclaration` | ✅ Pass |
| Alphabetical ordering | `FieldsAreSortedAlphabetically` | ✅ Pass |
| Generic ListView type | `WorksWithGenericListViewType` | ✅ Pass |
| Order property | `OrderIs220` | ✅ Pass |

## Field Declaration Contract

The transform emits fields in the form:
```csharp
private {Type} {controlId} = default!;
```

The `= default!;` null-forgiving initializer is mandatory — Blazor `@ref` targets
must be non-nullable fields but are assigned by the framework at render time.

## Recommendation

No further test work needed for this transform. Bishop's implementation is solid.
The duplicate-detection regex (`(?:private|protected|public|internal)\s+\w+.*\b{id}\s*[;=]`)
correctly handles the case where an existing declaration is present.


