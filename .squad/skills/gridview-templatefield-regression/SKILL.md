---
name: "gridview-templatefield-regression"
description: "Protect BWFC GridView TemplateField migrations from collapsing into BoundField output or losing nested controls"
domain: "migration-tooling"
confidence: "high"
source: "earned"
---

## Context
Use this when changing the Web Forms to Blazor CLI markup pipeline anywhere near GridView column transforms, template context handling, data-binding expression rewrites, or asp-prefix stripping. `TemplateField` preservation is a cross-transform behavior, so regressions can appear even when each individual transform still looks correct in isolation.

## Patterns
- Treat `TemplateField` preservation as an ordered pipeline contract:
  - `DisplayExpressionTransform`
  - `AspPrefixTransform`
  - `DataBindingAttributeTransform`
  - `AttributeStripTransform`
  - `GridViewColumnItemTypeTransform`
  - `TemplateContextTransform`
- Keep regression coverage at two levels:
  1. Transform-only markup assertions through `TestHelpers.CreateDefaultPipeline()`.
  2. Full-file migration assertions through `MigrationPipeline.ExecuteAsync()`.
- Cover these GridView shapes explicitly:
  - TemplateField with inner `TextBox`
  - TemplateField with inner `CheckBox`
  - TemplateField with inline display expression only
  - Mixed `BoundField` + `TemplateField` siblings
  - GridView with only TemplateField columns
- Assert both preservation and non-regression:
  - Expected `<TemplateField ...>` count remains stable
  - Inner controls survive as BWFC components (`<TextBox>`, `<CheckBox>`)
  - `ItemTemplate Context="Item"` is present
  - No unexpected fallback `<BoundField>` appears for template-only columns

## Examples
- `tests/BlazorWebFormsComponents.Cli.Tests/TransformUnit/GridViewColumnItemTypeTransformTests.cs`
- `tests/BlazorWebFormsComponents.Cli.Tests/PipelineIntegrationTests.cs`

## Anti-Patterns
- Testing only `GridViewColumnItemTypeTransform` in isolation after changing template-related transforms.
- Verifying only the first GridView column and assuming later TemplateField siblings survived.
- Accepting output that preserves the `TemplateField` shell but drops nested controls or display expressions.
- Changing transform order in `Program.cs` without mirroring the same coverage through `tests/BlazorWebFormsComponents.Cli.Tests/TestHelpers.cs`.
