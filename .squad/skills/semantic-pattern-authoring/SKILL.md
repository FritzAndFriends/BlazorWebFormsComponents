---
name: "semantic-pattern-authoring"
description: "Author isolated migration CLI semantic patterns without leaking behavior into Layer 1 transforms"
---

# Semantic Pattern Authoring

## Use when
- A recurring migrated page shape needs a post-transform rewrite.
- The change should stay in `src/BlazorWebFormsComponents.Cli/SemanticPatterns/`.
- You need targeted tests instead of broad pipeline edits.

## Pattern
1. Match on post-transform markup/code-behind, not original Web Forms syntax.
2. Keep wrapper extraction and markup surgery in subsystem-local helpers.
3. Preserve explicit TODO markers for app-specific behavior that the CLI cannot safely infer.
4. Prefer compile-safe SSR output over preserving non-runnable validator/postback trees.
5. Add isolated tests that assert both pattern IDs and rewritten markup shape.
6. For query-bound data pages, prefer `SelectMethod` → `SelectItems` scaffolds plus `[SupplyParameterFromQuery]` / `[Parameter]` component properties when the original Web Forms method relied on model-binding attributes.
7. For blank action pages, generate an SSR handler scaffold (`NavigationManager`, query-bound properties, redirect target, `OnParametersSet` TODO) instead of leaving inert migrated HTML.

## Current examples
- `pattern-query-details` — rewrites query-string / route-data `SelectMethod` pages to query-bound `SelectItems` stubs with `TODO(bwfc-query-details)`.
- `pattern-action-pages` — rewrites action-only redirect pages to SSR handler scaffolds with `TODO(bwfc-action-pages)`.
