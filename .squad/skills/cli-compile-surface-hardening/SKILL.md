---
name: "cli-compile-surface-hardening"
description: "Keep migrated Blazor output compiling when more Web Forms artifacts stay on the generated compile surface"
domain: "migration-tooling"
confidence: "high"
source: "earned"
---

## Context
Use this when the migration CLI starts emitting more generated `.razor.cs` files or copying more legacy source into the output project. The goal is to preserve deterministic L1 behavior while preventing compile failures from style analyzers, generic Razor component inference, or unresolved markup references.

## Patterns
- In generated migration projects, prefer `<EnforceCodeStyleInBuild>false</EnforceCodeStyleInBuild>` over broad warning suppression when copied legacy files would otherwise fail repo-level IDE analyzers.
- Add validator generic arguments with the BWFC component's actual generic parameter name, not an assumed `TValue` alias:
  - `RequiredFieldValidator` → `Type="string"`
  - `CompareValidator` / `RangeValidator` → `InputType="string"`
- Run markup-driven member stub generation after the main code-behind transforms, using transformed Razor markup (`metadata.MarkupContent`) as the source of truth.
- Stub only the deterministic missing-member shapes that are safe to infer mechanically:
  - `@MethodName()` → render-method stub returning `object?`
  - `@_fieldName` → private `object?` field
  - `OnClick="@HandlerName"` and similar → `void HandlerName(object? sender, EventArgs e)`
- Register new transforms in both production DI (`src\BlazorWebFormsComponents.Cli\Program.cs`) and `tests\BlazorWebFormsComponents.Cli.Tests\TestHelpers.cs` so the lightweight and full pipelines stay aligned.

## Examples
- `src\BlazorWebFormsComponents.Cli\Transforms\Markup\ValidatorGenericTypeTransform.cs`
- `src\BlazorWebFormsComponents.Cli\Transforms\CodeBehind\MarkupReferencedMemberStubTransform.cs`
- `tests\BlazorWebFormsComponents.Cli.Tests\TransformUnit\CompiledCodeBehindStubPipelineTests.cs`

## Anti-Patterns
- Suppressing all warnings/errors in the generated project when only code-style analyzers are the problem.
- Injecting `TValue="string"` into BWFC validators that actually expose `Type` or `InputType` generic parameters.
- Generating markup-reference stubs before markup transforms run, or reading original Web Forms markup instead of the transformed Razor output.
- Emitting placeholder stubs for every `@Identifier` token; restrict deterministic generation to the specific reference shapes you can classify safely.
